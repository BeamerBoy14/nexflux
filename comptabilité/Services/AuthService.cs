using comptabilité.Data;
using comptabilité.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace comptabilité.Services;

public class AuthService
{
    private readonly ComptabiliteDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly Dictionary<string, int> _userSessions = new Dictionary<string, int>();
    private static readonly Dictionary<string, string> _userNames = new Dictionary<string, string>();

    public AuthService(ComptabiliteDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetSessionKey()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return string.Empty;
        
        // Utiliser l'IP et le User-Agent comme clé de session simple
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        return $"{ip}_{userAgent}";
    }

    public async Task<AuthResult> AuthentifierAsync(string nomUtilisateur, string motDePasse)
    {
        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.NomUtilisateur == nomUtilisateur && u.EstActif);

        if (utilisateur == null)
            return new AuthResult { Success = false, ErrorMessage = "Utilisateur non trouvé" };

        // Vérifier le mot de passe
        var motDePasseHash = HashPassword(motDePasse);
        if (utilisateur.MotDePasseHash != motDePasseHash)
            return new AuthResult { Success = false, ErrorMessage = "Mot de passe incorrect" };

        // Mettre à jour la dernière connexion
        utilisateur.DerniereConnexion = DateTime.Now;
        await _context.SaveChangesAsync();

        // Stocker dans un dictionnaire en mémoire (simple mais fonctionne)
        var sessionKey = GetSessionKey();
        if (!string.IsNullOrEmpty(sessionKey))
        {
            _userSessions[sessionKey] = utilisateur.Id;
            _userNames[sessionKey] = utilisateur.NomUtilisateur;
        }

        return new AuthResult
        {
            Success = true,
            UserId = utilisateur.Id,
            Username = utilisateur.NomUtilisateur
        };
    }

    public void Deconnecter()
    {
        try
        {
            var sessionKey = GetSessionKey();
            if (!string.IsNullOrEmpty(sessionKey))
            {
                _userSessions.Remove(sessionKey);
                _userNames.Remove(sessionKey);
            }
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
            var sessionKey = GetSessionKey();
            if (string.IsNullOrEmpty(sessionKey) || !_userSessions.TryGetValue(sessionKey, out var userId))
                return null;
            
            return userId;
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
            var sessionKey = GetSessionKey();
            if (string.IsNullOrEmpty(sessionKey) || !_userNames.TryGetValue(sessionKey, out var username))
                return null;
            
            return username;
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
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
