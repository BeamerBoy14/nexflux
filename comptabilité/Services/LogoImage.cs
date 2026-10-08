namespace comptabilité.Services;

public static class LogoImage
{
    public const int MaxBytes = 2 * 1024 * 1024;

    // Identifie le type réel à partir des premiers octets (le type annoncé par le navigateur n'est pas fiable).
    // Le SVG est volontairement refusé : il peut contenir des scripts.
    public static bool TryGetContentType(ReadOnlySpan<byte> data, out string contentType)
    {
        contentType = string.Empty;

        if (data.Length >= 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
        {
            contentType = "image/png";
        }
        else if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
        {
            contentType = "image/jpeg";
        }
        else if (data.Length >= 6 && data[0] == (byte)'G' && data[1] == (byte)'I' && data[2] == (byte)'F' && data[3] == (byte)'8')
        {
            contentType = "image/gif";
        }
        else if (data.Length >= 12
                 && data[0] == (byte)'R' && data[1] == (byte)'I' && data[2] == (byte)'F' && data[3] == (byte)'F'
                 && data[8] == (byte)'W' && data[9] == (byte)'E' && data[10] == (byte)'B' && data[11] == (byte)'P')
        {
            contentType = "image/webp";
        }

        return contentType.Length > 0;
    }
}
