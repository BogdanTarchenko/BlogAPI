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
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityUser> CommunityUsers { get; set; }
    
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
        
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });
        
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.ReadingTime).IsRequired();
            entity.Property(e => e.CreateTime).IsRequired();
            
            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Community)
                .WithMany(c => c.Posts)
                .HasForeignKey(e => e.CommunityId);
        });
        
        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.TagId });
            
            entity.HasOne(e => e.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(e => e.PostId);
                
            entity.HasOne(e => e.Tag)
                .WithMany()
                .HasForeignKey(e => e.TagId);
        });
        
        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.UserId });
            entity.Property(e => e.CreateTime).IsRequired();
            
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(e => e.PostId);
                
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(5000);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.IsDeleted).IsRequired();

            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.CreateTime).IsRequired();
            entity.Property(e => e.IsClosed).IsRequired();
            entity.Property(e => e.SubscribersCount).IsRequired().HasDefaultValue(0);

            entity.HasMany(e => e.CommunityUsers)
                  .WithOne(cu => cu.Community)
                  .HasForeignKey(cu => cu.CommunityId);

            entity.HasMany(e => e.Posts)
                  .WithOne(p => p.Community)
                  .HasForeignKey(p => p.CommunityId);
        });

        modelBuilder.Entity<CommunityUser>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CommunityId });

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Community)
                .WithMany(c => c.CommunityUsers)
                .HasForeignKey(e => e.CommunityId);
        });
    }
}