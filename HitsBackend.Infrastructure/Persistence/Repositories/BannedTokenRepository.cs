using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

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
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var expirationTime = claims.ValidTo.ToUniversalTime();
        
        var bannedToken = new BannedToken
        {
            Token = token,
            ExpirationTime = expirationTime
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
        var expiredTokens = await _context.BannedTokens
            .Where(t => t.ExpirationTime < DateTime.UtcNow)
            .ToListAsync();
        
        _context.BannedTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}