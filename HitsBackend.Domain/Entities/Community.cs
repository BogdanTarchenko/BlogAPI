namespace HitsBackend.Domain.Entities;

public class Community
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsClosed { get; set; }
    public int SubscribersCount { get; set; } = 0;
    public DateTime CreateTime { get; set; }
    
    public ICollection<CommunityUser> CommunityUsers { get; set; } = new List<CommunityUser>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}