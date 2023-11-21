using System.ComponentModel.DataAnnotations;

namespace Y.Infrastructure.Tables;

public class Profile
{
    [Key]
    public Guid Guid { get; set; }

    [MaxLength(75)]
    public string Quote { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    // TODO: Add profile image
}
