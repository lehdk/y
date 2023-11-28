using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Y.Infrastructure.Tables;

public class Posts
{
    [Key]
    public Guid PostId{ get; set; }

    [MaxLength(75)]
    public string Headline { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    [DefaultValue("getutcdate()")]
    public DateTime CreatedAt { get; set; }
}