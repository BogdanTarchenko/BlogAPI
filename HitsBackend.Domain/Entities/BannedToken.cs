namespace HitsBackend.Domain.Entities;

public class BannedToken
{
    public Guid Id { get; set; } 
    public required string Token { get; set; }
}