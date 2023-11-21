using System.ComponentModel.DataAnnotations;

namespace Y.Domain.Models;

public class YUser
{
    public Guid Guid { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public YProfile Profile { get; set; }

    public YUser(YProfile profile) 
    {
        Profile = profile;
    }
}
