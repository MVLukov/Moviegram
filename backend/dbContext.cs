using Microsoft.EntityFrameworkCore;
using moviesApi.Models;

namespace moviesApi;

public class dbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }

    public dbContext(DbContextOptions<dbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>()
            .Property(c => c.Role)
            .HasConversion<int>();

        base.OnModelCreating(modelBuilder);

    }
}