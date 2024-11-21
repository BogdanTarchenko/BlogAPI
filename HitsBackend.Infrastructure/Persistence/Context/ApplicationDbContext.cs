using HitsBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<BannedToken> BannedTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired();
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.Gender).IsRequired();
        });
        
        modelBuilder.Entity<BannedToken>(entity =>
        {
            entity.HasKey(e => e.Token);
            entity.Property(e => e.ExpirationTime).IsRequired();
        });
    }
}