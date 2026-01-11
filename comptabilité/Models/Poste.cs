using System.ComponentModel.DataAnnotations;

namespace comptabilité.Models;

public class Poste
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom du poste est obligatoire")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string Nom { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool PeutAvoirVoiture { get; set; } = false; // Indique si ce poste peut être associé à une voiture

    public DateTime DateCreation { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

