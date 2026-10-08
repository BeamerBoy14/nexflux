using System.ComponentModel.DataAnnotations;

namespace comptabilité.Models;

public class Voiture
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de la voiture est obligatoire")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string Nom { get; set; } = string.Empty; // Ex: Mustang, Corvette

    public string? Immatriculation { get; set; }

    [Range(1900, 2100, ErrorMessage = "L'année doit être comprise entre 1900 et 2100")]
    public int? Annee { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Le kilométrage doit être compris entre 0 et 10 000 000")]
    public int? Kilometrage { get; set; }

    public string? Description { get; set; }

    // Date du dernier envoi de logo (null = pas de logo) ; sert aussi à rafraîchir le cache du navigateur
    public DateTime? LogoUpdatedAt { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;

    // Relation avec Utilisateur
    public int? UtilisateurId { get; set; }
    public Utilisateur? Utilisateur { get; set; }

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public VoitureLogo? Logo { get; set; }
}

