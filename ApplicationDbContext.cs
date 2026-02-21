using Microsoft.EntityFrameworkCore;
using Obrasheniya.Domain.Entities;

namespace Obrasheniya.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Obrashenie> Obrasheniya => Set<Obrashenie>();
    public DbSet<ObrashenieResponse> Responses => Set<ObrashenieResponse>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role).HasConversion<string>();
        });
        
        modelBuilder.Entity<Obrashenie>(entity =>
        {
            entity.HasIndex(o => o.Status);
            entity.Property(o => o.Status).HasConversion<string>();
            entity.HasOne(o => o.User)
                  .WithMany(u => u.Obrasheniya)
                  .HasForeignKey(o => o.UserId);
        });
    }
}