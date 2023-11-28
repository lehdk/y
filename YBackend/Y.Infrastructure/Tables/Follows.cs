using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Y.Infrastructure.Tables;

public class Follows
{
    [Key]
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    public Guid Follow { get; set; }
    
    [DefaultValue("getutcdate()")]
    public DateTime CreatedAt { get; set; }
}