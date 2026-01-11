namespace comptabilité.Models;

public class LigneFacture
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Quantite { get; set; } = 1;
    public decimal PrixUnitaire { get; set; }
    public decimal MontantHT { get; set; }
    public decimal TauxTVA { get; set; } = 20;
    public decimal MontantTTC { get; set; }
    
    // Relations
    public int FactureId { get; set; }
    public Facture Facture { get; set; } = null!;
}






