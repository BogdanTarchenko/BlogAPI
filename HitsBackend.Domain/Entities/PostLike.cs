namespace HitsBackend.Domain.Entities;

public class PostLike
{
    public required Guid PostId { get; set; }
    public required Post Post { get; set; }
    
    public required Guid UserId { get; set; }
    public required User User { get; set; }
    
    public required DateTime CreateTime { get; set; }
} 