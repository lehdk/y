using System.ComponentModel.DataAnnotations;

namespace Y.Domain.Models;

public class YProfile
{
    public Guid Guid { get; set; }

    [MaxLength(75)]
    public string Quote { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
