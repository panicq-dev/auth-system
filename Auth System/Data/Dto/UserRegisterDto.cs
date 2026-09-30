using System.ComponentModel.DataAnnotations;

namespace Auth_System.Data.Dto;

public class UserRegisterDto
{
    
    public required string UserName { get; set; }
    public required string Password { get; set; }
    [Compare("Password")]
    public required string RePassword { get; set; }
}