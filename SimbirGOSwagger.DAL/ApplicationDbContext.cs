using Microsoft.EntityFrameworkCore;
using SimbirGOSwagger.Domain.Entity;

namespace SimbirGOSwagger.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> User { get; set; }
    public DbSet<Transport> Transport { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.HasData(new User()
            {
                Id = 1,
                Username = "Admin",
                Balance = 0,
                IsAdmin = true,
                Password = "Admin"
            });
            
            builder.Property(x => x.Password).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Username).HasMaxLength(30).IsRequired();
        });
    }
}