using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Y.Infrastructure.Tables;

public class User
{
    [Key]
    public Guid Guid { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [DefaultValue("getutcdate()")]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(null)]
    public DateTime? LastLogin { get; set; }

    public Guid ProfileId { get; set; }
    
    public PasswordSalts PasswordSalt { get; set; }
}