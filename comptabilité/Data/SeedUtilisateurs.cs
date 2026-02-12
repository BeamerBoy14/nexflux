using comptabilité.Models;
using comptabilité.Services;
using Microsoft.EntityFrameworkCore;

namespace comptabilité.Data;

public static class SeedUtilisateurs
{
    public static async Task SeedAsync(ComptabiliteDbContext context)
    {
        // Vérifier si des utilisateurs existent déjà
        if (await context.Utilisateurs.AnyAsync())
        {
            return; // Les utilisateurs existent déjà
        }

        // Créer les 2 utilisateurs initiaux
        var utilisateurs = new[]
        {
            new Utilisateur
            {
                NomUtilisateur = "TransEraLegend",
                MotDePasseHash = AuthService.HashPassword("TransEra2024!"), // Mot de passe par défaut
                Email = "transeralegend@nexflux.com",
                NomComplet = "TransEra Legend",
                EstActif = true,
                DateCreation = DateTime.Now
            },
            new Utilisateur
            {
                NomUtilisateur = "Entreprise2",
                MotDePasseHash = AuthService.HashPassword("Entreprise2024!"), // Mot de passe par défaut
                Email = "entreprise2@nexflux.com",
                NomComplet = "Entreprise 2",
                EstActif = true,
                DateCreation = DateTime.Now
            }
        };

        context.Utilisateurs.AddRange(utilisateurs);
        await context.SaveChangesAsync();
    }
}
