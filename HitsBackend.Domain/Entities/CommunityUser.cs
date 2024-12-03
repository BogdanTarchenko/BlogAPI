using HitsBackend.Domain.Enums;

namespace HitsBackend.Domain.Entities;

public class CommunityUser
{
    public Guid UserId { get; set; }
    public Guid CommunityId { get; set; }
    public CommunityRole Role { get; set; }
    
    public User User { get; set; }
    public Community Community { get; set; }
}