namespace comptabilité.Models;

public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Prenom { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public string? CodePostal { get; set; }
    public string? Ville { get; set; }
    public string? NumeroIdd { get; set; }
    public string? NumeroPermis { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string? IdTel { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Facture> Factures { get; set; } = new List<Facture>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}


