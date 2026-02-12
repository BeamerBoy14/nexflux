using comptabilité.Models;
using Microsoft.EntityFrameworkCore;

namespace comptabilité.Data;

public static class AssignDataToTransEraLegend
{
    public static async Task AssignExistingDataAsync(ComptabiliteDbContext context)
    {
        // Trouver l'utilisateur TransEraLegend
        var utilisateur = await context.Utilisateurs
            .FirstOrDefaultAsync(u => u.NomUtilisateur == "TransEraLegend");

        if (utilisateur == null)
        {
            Console.WriteLine("Utilisateur TransEraLegend non trouvé. Les données ne seront pas assignées.");
            return;
        }

        var userId = utilisateur.Id;
        var updated = 0;

        // Assigner tous les Clients sans UtilisateurId
        var clientsSansUtilisateur = await context.Clients
            .Where(c => c.UtilisateurId == null)
            .ToListAsync();
        foreach (var client in clientsSansUtilisateur)
        {
            client.UtilisateurId = userId;
        }
        updated += clientsSansUtilisateur.Count;

        // Assigner toutes les Transactions sans UtilisateurId
        var transactionsSansUtilisateur = await context.Transactions
            .Where(t => t.UtilisateurId == null)
            .ToListAsync();
        foreach (var transaction in transactionsSansUtilisateur)
        {
            transaction.UtilisateurId = userId;
        }
        updated += transactionsSansUtilisateur.Count;

        // Assigner toutes les Reservations sans UtilisateurId
        var reservationsSansUtilisateur = await context.Reservations
            .Where(r => r.UtilisateurId == null)
            .ToListAsync();
        foreach (var reservation in reservationsSansUtilisateur)
        {
            reservation.UtilisateurId = userId;
        }
        updated += reservationsSansUtilisateur.Count;

        // Assigner tous les Postes sans UtilisateurId
        var postesSansUtilisateur = await context.Postes
            .Where(p => p.UtilisateurId == null)
            .ToListAsync();
        foreach (var poste in postesSansUtilisateur)
        {
            poste.UtilisateurId = userId;
        }
        updated += postesSansUtilisateur.Count;

        // Assigner toutes les Voitures sans UtilisateurId
        var voituresSansUtilisateur = await context.Voitures
            .Where(v => v.UtilisateurId == null)
            .ToListAsync();
        foreach (var voiture in voituresSansUtilisateur)
        {
            voiture.UtilisateurId = userId;
        }
        updated += voituresSansUtilisateur.Count;

        // Assigner toutes les Factures sans UtilisateurId
        var facturesSansUtilisateur = await context.Factures
            .Where(f => f.UtilisateurId == null)
            .ToListAsync();
        foreach (var facture in facturesSansUtilisateur)
        {
            facture.UtilisateurId = userId;
        }
        updated += facturesSansUtilisateur.Count;

        if (updated > 0)
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"Assigné {updated} entités à l'utilisateur TransEraLegend (ID: {userId})");
        }
        else
        {
            Console.WriteLine("Aucune donnée à assigner (toutes les données ont déjà un UtilisateurId)");
        }
    }
}
