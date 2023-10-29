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
}