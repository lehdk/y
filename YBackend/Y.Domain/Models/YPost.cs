using System.ComponentModel.DataAnnotations;

namespace Y.Domain.Models;

public class YPost
{
    public Guid Id { get; set; }

    [MaxLength(75)]
    public string Headline { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
