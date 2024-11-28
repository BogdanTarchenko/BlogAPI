using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllWithStatsAsync()
    {
        return await _context.Users
            .OrderByDescending(u => u.CreateTime)
            .ToListAsync();
    }
    
    public async Task IncrementPostsCountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.PostsCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementLikesCountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LikesCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DecrementLikesCountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null && user.LikesCount > 0)
        {
            user.LikesCount--;
            await _context.SaveChangesAsync();
        }
    }
}