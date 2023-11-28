using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Y.Infrastructure.Tables;

public class PostReactions
{
    [Key]
    public Guid UserId { get; set; }

    public String Reactions { get; set; }
    
    public User User { get; set; }
}