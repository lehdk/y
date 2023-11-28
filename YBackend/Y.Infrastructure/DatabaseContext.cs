using Microsoft.EntityFrameworkCore;
using Y.Infrastructure.Extensions;
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
    
    public DbSet<Posts> Posts { get; set; }
    
    public DbSet<PostReactions> PostReactions { get; set; }
    
    public DbSet<PostComments> PostComments { get; set; }
    
    public DbSet<Follows> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(x => x.PasswordSalt)
            .WithOne(x => x.User)
            .HasForeignKey<PasswordSalts>(x => x.UserId);

        modelBuilder.Entity<Posts>()
            .HasOne(x => x.User)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<PostReactions>()
            .HasOne(x => x.User)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<PostComments>()
            .HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .HasForeignKey(x => x.SuperComment)
            .HasForeignKey(x => x.PostId);

        modelBuilder.Entity<Follows>()
            .HasOne(x => x.User)
            .WithMany(x => x.Followers)
            .HasForeignKey(x => x.UserId);
        
        base.OnModelCreating(modelBuilder);
    }

}
