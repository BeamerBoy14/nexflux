using System.ComponentModel.DataAnnotations;

namespace comptabilité.Models;

public class Voiture
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de la voiture est obligatoire")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string Nom { get; set; } = string.Empty; // Ex: Mustang, Corvette

    public string? Immatriculation { get; set; }

    public string? Description { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

