using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BannedTokenRepository : IBannedTokenRepository
{
    private readonly ApplicationDbContext _context;

    public BannedTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(string token)
    {
        var bannedToken = new BannedToken
        {
            Id = Guid.NewGuid(),
            Token = token
        };
        
        await _context.BannedTokens.AddAsync(bannedToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTokenBannedAsync(string token)
    {
        return await _context.BannedTokens.AnyAsync(t => t.Token == token);
    }

    public async Task ClearAllAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"BannedTokens\"");
    }
}