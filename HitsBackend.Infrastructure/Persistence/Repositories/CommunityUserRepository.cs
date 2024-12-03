using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CommunityUserRepository : ICommunityUserRepository
{
    private readonly ApplicationDbContext _context;

    public CommunityUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsUserSubscribedAsync(Guid communityId, Guid userId)
    {
        return await _context.CommunityUsers.AnyAsync(cu => cu.CommunityId == communityId && cu.UserId == userId);
    }

    public async Task AddUserToCommunityAsync(Guid communityId, Guid userId)
    {
        var communityUser = new CommunityUser { CommunityId = communityId, UserId = userId };
        await _context.CommunityUsers.AddAsync(communityUser);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserFromCommunityAsync(Guid communityId, Guid userId)
    {
        var communityUser = await _context.CommunityUsers
            .FirstOrDefaultAsync(cu => cu.CommunityId == communityId && cu.UserId == userId);

        if (communityUser != null)
        {
            _context.CommunityUsers.Remove(communityUser);
            await _context.SaveChangesAsync();
        }
    }
} 