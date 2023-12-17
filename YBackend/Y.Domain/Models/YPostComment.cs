using System.ComponentModel.DataAnnotations;

namespace Y.Domain.Models;

public class YPostComment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    [MaxLength(250)]
    public string Text { get; set; } = string.Empty;

    public Guid? SuperComment { get; set; }

    public DateTime CreatedAt { get; set; }
}
