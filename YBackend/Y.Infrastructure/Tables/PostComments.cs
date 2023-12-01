using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Y.Infrastructure.Tables;

public class PostComments
{
    [Key]
    public Guid CommentId { get; set; }
    
    public Guid PostId { get; set; }

    [MaxLength(250)]
    public string Text { get; set; } = string.Empty;

    public PostComments? SuperComment { get; set; }
    public Guid? SuperCommentId { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    [DefaultValue("getutcdate()")]
    public DateTime CreatedAt { get; set; }
}