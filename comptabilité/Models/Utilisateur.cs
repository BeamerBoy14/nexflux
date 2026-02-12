using System.ComponentModel.DataAnnotations;

namespace comptabilité.Models;

public class Utilisateur
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string NomUtilisateur { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string MotDePasseHash { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? NomComplet { get; set; }

    public bool EstActif { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.Now;

    public DateTime? DerniereConnexion { get; set; }

    // Navigation properties
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Facture> Factures { get; set; } = new List<Facture>();
    public ICollection<Poste> Postes { get; set; } = new List<Poste>();
    public ICollection<Voiture> Voitures { get; set; } = new List<Voiture>();
}
