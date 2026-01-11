using System.ComponentModel.DataAnnotations;

namespace comptabilité.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    [Required]
    public DateTime DateDebut { get; set; } = DateTime.Now;

    [Required]
    public DateTime DateFin { get; set; } = DateTime.Now;

    [Required]
    [StringLength(50)]
    public string Statut { get; set; } = "En attente";

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}


