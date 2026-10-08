using comptabilité.Models;
using comptabilité.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace comptabilité.Data;

public static class SeedUtilisateurs
{
    private sealed record Compte(string NomUtilisateur, string CleMotDePasse, string Email, string NomComplet);

    private static readonly Compte[] Comptes =
    {
        new("TransEraLegend", "Seed:TransEraLegendPassword", "transeralegend@nexflux.com", "TransEra Legend"),
        new("Entreprise2", "Seed:Entreprise2Password", "entreprise2@nexflux.com", "Entreprise 2")
    };

    // Les mots de passe ne sont jamais écrits dans le code (le dépôt est public) : ils viennent de la
    // configuration (variables d'environnement Seed__TransEraLegendPassword / Seed__Entreprise2Password,
    // ou user-secrets en local). Quand ils sont renseignés, la configuration fait foi : le compte est créé
    // s'il n'existe pas, et son mot de passe est resynchronisé s'il ne correspond plus.
    // Sans valeur, rien n'est créé ni modifié (les comptes existants gardent leur mot de passe).
    public static async Task SeedAsync(ComptabiliteDbContext context, IConfiguration configuration, ILogger logger)
    {
        var hasher = new PasswordHasher<Utilisateur>();
        var modifie = false;

        foreach (var compte in Comptes)
        {
            var motDePasse = configuration[compte.CleMotDePasse];
            var utilisateur = await context.Utilisateurs
                .FirstOrDefaultAsync(u => u.NomUtilisateur == compte.NomUtilisateur);

            if (string.IsNullOrWhiteSpace(motDePasse))
            {
                if (utilisateur is null)
                {
                    logger.LogWarning("Compte {Compte} non créé : renseignez {Cle}.", compte.NomUtilisateur, compte.CleMotDePasse);
                }

                continue;
            }

            if (utilisateur is null)
            {
                context.Utilisateurs.Add(new Utilisateur
                {
                    NomUtilisateur = compte.NomUtilisateur,
                    MotDePasseHash = AuthService.HashPassword(motDePasse),
                    Email = compte.Email,
                    NomComplet = compte.NomComplet,
                    EstActif = true,
                    DateCreation = DateTime.Now
                });
                modifie = true;
                logger.LogInformation("Compte {Compte} créé.", compte.NomUtilisateur);
            }
            else if (!MotDePasseCorrespond(hasher, utilisateur, motDePasse))
            {
                utilisateur.MotDePasseHash = AuthService.HashPassword(motDePasse);
                modifie = true;
                logger.LogInformation("Mot de passe du compte {Compte} resynchronisé avec la configuration.", compte.NomUtilisateur);
            }
        }

        if (modifie)
        {
            await context.SaveChangesAsync();
        }
    }

    private static bool MotDePasseCorrespond(PasswordHasher<Utilisateur> hasher, Utilisateur utilisateur, string motDePasse)
    {
        try
        {
            return hasher.VerifyHashedPassword(utilisateur, utilisateur.MotDePasseHash, motDePasse) != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            // Ancien format (SHA256) ou valeur illisible : considéré comme différent, donc remplacé
            return false;
        }
    }
}
