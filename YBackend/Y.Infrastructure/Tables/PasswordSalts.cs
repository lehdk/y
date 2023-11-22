using System.ComponentModel.DataAnnotations;

namespace Y.Infrastructure.Tables;

public class PasswordSalts
{
    
    public User User { get; set; }
    
    [Key]
    public Guid UserId { get; set; }

    public string Salt { get; set; }
}