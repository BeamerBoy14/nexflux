using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace comptabilité.Models;

public class Transaction : IValidatableObject
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La date est obligatoire")]
    public DateTime Date { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "Le montant est obligatoire")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
    public decimal Montant { get; set; }
    
    [StringLength(200, ErrorMessage = "Le libellé ne peut pas dépasser 200 caractères")]
    public string Libelle { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Le type est obligatoire")]
    public string Type { get; set; } = "Dépense"; // Dépense ou Gain
    
    // Relations optionnelles
    public int? PosteId { get; set; }
    public Poste? Poste { get; set; }
    
    public int? VoitureId { get; set; }
    public Voiture? Voiture { get; set; }
    
    public int? ClientId { get; set; }
    public Client? Client { get; set; }

    public int? ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(Type, "Gain", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(Libelle))
        {
            yield return new ValidationResult(
                "Le libellé est obligatoire pour un gain.",
                new[] { nameof(Libelle) });
        }
    }
}
