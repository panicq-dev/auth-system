namespace Auth_System.Data.Dto;

public class AuthInfoResponse
{
    public string AcessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}