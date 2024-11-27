namespace HitsBackend.Domain.Entities;

public class PostTag
{
    public required Guid PostId { get; set; }
    public required Post Post { get; set; }
    
    public required Guid TagId { get; set; }
    public required Tag Tag { get; set; }
} 