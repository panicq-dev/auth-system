namespace Auth_System.Data.Dto;

public class AuthInfo
{
    public string AcessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string RefreshTokenHash { get; set; }
}