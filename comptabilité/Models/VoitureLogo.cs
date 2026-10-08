namespace comptabilité.Models;

public class VoitureLogo
{
    public int VoitureId { get; set; }
    public Voiture? Voiture { get; set; }

    public byte[] Data { get; set; } = Array.Empty<byte>();

    public string ContentType { get; set; } = "image/png";
}
