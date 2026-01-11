namespace comptabilité.Models;

public class Facture
{
    public int Id { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public DateTime DateFacture { get; set; } = DateTime.Now;
    public DateTime? DateEcheance { get; set; }
    public decimal MontantHT { get; set; }
    public decimal TauxTVA { get; set; } = 20; // 20% par défaut
    public decimal MontantTVA { get; set; }
    public decimal MontantTTC { get; set; }
    public string Statut { get; set; } = "Brouillon"; // Brouillon, Envoyée, Payée, Impayée
    public string? Notes { get; set; }

    // Relations
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    // Navigation properties
    public ICollection<LigneFacture> LignesFacture { get; set; } = new List<LigneFacture>();
}






