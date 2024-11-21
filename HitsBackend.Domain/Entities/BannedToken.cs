namespace HitsBackend.Domain.Entities;

public class BannedToken
{
    public required string Token { get; set; }
    public required DateTime ExpirationTime { get; set; }
}