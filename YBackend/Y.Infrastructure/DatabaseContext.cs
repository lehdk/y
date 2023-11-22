using Microsoft.EntityFrameworkCore;
using Y.Infrastructure.Tables;

namespace Y.Infrastructure;

public class DatabaseContext : DbContext
{

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    
    public DbSet<PasswordSalts> PasswordSalts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(x => x.PasswordSalt)
            .WithOne(x => x.User)
            .HasForeignKey<PasswordSalts>(x => x.UserId);
        base.OnModelCreating(modelBuilder);
    }

}
