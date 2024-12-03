using HitsBackend.Domain.Entities;

namespace HitsBackend.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int ReadingTime { get; set; }
    public string? Image { get; set; }
    public required DateTime CreateTime { get; set; }
    
    public required Guid AuthorId { get; set; }
    public required User Author { get; set; }
    
    public Guid? AddressId { get; set; }
    
    public Guid? CommunityId { get; set; }
    public Community? Community { get; set; }

    public string? CommunityName { get; set; }
    
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
} 