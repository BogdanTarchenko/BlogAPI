using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CommunityRepository : ICommunityRepository
{
    private readonly ApplicationDbContext _context;

    public CommunityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Community?> GetByIdAsync(Guid id)
    {
        return await _context.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Community>> GetAllAsync()
    {
        return await _context.Communities.ToListAsync();
    }

    public async Task AddAsync(Community community)
    {
        await _context.Communities.AddAsync(community);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Community community)
    {
        _context.Communities.Update(community);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var community = await GetByIdAsync(id);
        if (community != null)
        {
            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();
        }
    }
}