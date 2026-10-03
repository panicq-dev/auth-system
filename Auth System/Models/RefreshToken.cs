using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_System.Models;

public class RefreshToken
{
    public int Id { get; set; }

    [Required]
    [MaxLength(450)]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    [Required]
    [MaxLength(512)]
    public string RefreshTokenHash { get; set; } = string.Empty;

    public DateTime? Expires { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? RevokeAt { get; set; }
}