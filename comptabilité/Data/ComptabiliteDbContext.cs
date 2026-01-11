using comptabilité.Models;
using Microsoft.EntityFrameworkCore;

namespace comptabilité.Data;

public class ComptabiliteDbContext : DbContext
{
    public ComptabiliteDbContext(DbContextOptions<ComptabiliteDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Facture> Factures { get; set; }
    public DbSet<LigneFacture> LignesFacture { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Poste> Postes { get; set; }
    public DbSet<Voiture> Voitures { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration Client
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telephone).HasMaxLength(20);
            entity.Property(e => e.Adresse).HasMaxLength(200);
            entity.Property(e => e.CodePostal).HasMaxLength(10);
            entity.Property(e => e.Ville).HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configuration Facture
        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroFacture).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MontantHT).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TauxTVA).HasColumnType("decimal(5,2)");
            entity.Property(e => e.MontantTVA).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MontantTTC).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Statut).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.NumeroFacture).IsUnique();

            entity.HasOne(e => e.Client)
                .WithMany(c => c.Factures)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration LigneFacture
        modelBuilder.Entity<LigneFacture>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Libelle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Quantite).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PrixUnitaire).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MontantHT).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TauxTVA).HasColumnType("decimal(5,2)");
            entity.Property(e => e.MontantTTC).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Facture)
                .WithMany(f => f.LignesFacture)
                .HasForeignKey(e => e.FactureId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Poste
        modelBuilder.Entity<Poste>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nom).IsUnique();
        });

        // Configuration Voiture
        modelBuilder.Entity<Voiture>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Immatriculation).HasMaxLength(20);
        });

        // Configuration Reservation
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Statut).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Client)
                .WithMany(c => c.Reservations)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Libelle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Montant).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Date);

            entity.HasOne(e => e.Poste)
                .WithMany(p => p.Transactions)
                .HasForeignKey(e => e.PosteId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Voiture)
                .WithMany(v => v.Transactions)
                .HasForeignKey(e => e.VoitureId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Client)
                .WithMany(c => c.Transactions)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Reservation)
                .WithMany(r => r.Transactions)
                .HasForeignKey(e => e.ReservationId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}


