namespace HitsBackend.Application.Common.Interfaces;

public interface IBannedTokenRepository
{
    Task AddAsync(string token);
    Task<bool> IsTokenBannedAsync(string token);
    Task ClearAllAsync();
}