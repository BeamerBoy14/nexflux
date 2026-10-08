using comptabilité.Data;
using comptabilité.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace comptabilité.Services;

public class AuthService
{
    private const string SessionUserIdKey = "UserId";
    private const string SessionUserNameKey = "UserName";

    // Tentatives échouées par nom d'utilisateur : protection basique contre le brute-force.
    private static readonly Dictionary<string, (int Attempts, DateTime BlockedUntil)> _loginAttempts = new();
    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private static readonly PasswordHasher<Utilisateur> _passwordHasher = new();

    private readonly ComptabiliteDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(ComptabiliteDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResult> AuthentifierAsync(string nomUtilisateur, string motDePasse)
    {
        lock (_loginAttempts)
        {
            if (_loginAttempts.TryGetValue(nomUtilisateur, out var tentative) && tentative.BlockedUntil > DateTime.Now)
            {
                var minutesRestantes = Math.Ceiling((tentative.BlockedUntil - DateTime.Now).TotalMinutes);
                return new AuthResult { Success = false, ErrorMessage = $"Trop de tentatives. Réessayez dans {minutesRestantes} min." };
            }
        }

        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.NomUtilisateur == nomUtilisateur && u.EstActif);

        if (utilisateur == null || !VerifierMotDePasse(utilisateur, motDePasse))
        {
            EnregistrerTentativeEchouee(nomUtilisateur);
            return new AuthResult { Success = false, ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect" };
        }

        lock (_loginAttempts)
        {
            _loginAttempts.Remove(nomUtilisateur);
        }

        utilisateur.DerniereConnexion = DateTime.Now;
        await _context.SaveChangesAsync();

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Session.SetInt32(SessionUserIdKey, utilisateur.Id);
            httpContext.Session.SetString(SessionUserNameKey, utilisateur.NomUtilisateur);

            // Le login est déclenché par un clic (circuit SignalR Blazor), pas par une requête HTTP
            // classique : le commit automatique de la session (lié au cycle de vie de la réponse HTTP)
            // n'a donc pas lieu. Sans ce commit explicite, les valeurs ci-dessus restent uniquement en
            // mémoire locale et ne sont jamais écrites dans le cache de session.
            await httpContext.Session.CommitAsync();
        }

        return new AuthResult
        {
            Success = true,
            UserId = utilisateur.Id,
            Username = utilisateur.NomUtilisateur
        };
    }

    private static void EnregistrerTentativeEchouee(string nomUtilisateur)
    {
        lock (_loginAttempts)
        {
            var (attempts, _) = _loginAttempts.TryGetValue(nomUtilisateur, out var existant) ? existant : (0, DateTime.MinValue);
            attempts++;
            var blockedUntil = attempts >= MaxAttempts ? DateTime.Now.Add(LockoutDuration) : DateTime.MinValue;
            _loginAttempts[nomUtilisateur] = (attempts, blockedUntil);
        }
    }

    private bool VerifierMotDePasse(Utilisateur utilisateur, string motDePasse)
    {
        PasswordVerificationResult resultat;
        try
        {
            resultat = _passwordHasher.VerifyHashedPassword(utilisateur, utilisateur.MotDePasseHash, motDePasse);
        }
        catch (FormatException)
        {
            resultat = PasswordVerificationResult.Failed;
        }

        if (resultat == PasswordVerificationResult.Failed)
        {
            // Pas reconnu par le nouveau hasher : peut-être l'ancien format SHA256 (pré-migration).
            // VerifyHashedPassword renvoie simplement "Failed" pour un format inconnu (pas d'exception),
            // donc ce fallback doit être vérifié ici, pas seulement dans le catch.
            if (LegacyHashPassword(motDePasse) != utilisateur.MotDePasseHash)
                return false;

            // Mot de passe correct avec l'ancien hash : migration vers le nouveau format.
            utilisateur.MotDePasseHash = HashPassword(motDePasse);
            return true;
        }

        if (resultat == PasswordVerificationResult.SuccessRehashNeeded)
            utilisateur.MotDePasseHash = HashPassword(motDePasse);

        return true;
    }

    public void Deconnecter()
    {
        try
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }
        catch
        {
            // Ignorer les erreurs de déconnexion
        }
    }

    public int? GetUtilisateurId()
    {
        try
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32(SessionUserIdKey);
        }
        catch
        {
            return null;
        }
    }

    public string? GetUtilisateurNom()
    {
        try
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(SessionUserNameKey);
        }
        catch
        {
            return null;
        }
    }

    public bool EstConnecte()
    {
        try
        {
            var userId = GetUtilisateurId();
            return userId.HasValue && userId.Value > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Utilisateur?> GetUtilisateurActuelAsync()
    {
        var id = GetUtilisateurId();
        if (!id.HasValue)
            return null;

        return await _context.Utilisateurs.FindAsync(id.Value);
    }

    public static string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    private static string LegacyHashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
