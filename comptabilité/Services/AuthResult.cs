namespace comptabilité.Services;

public class AuthResult
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? ErrorMessage { get; set; }
}
