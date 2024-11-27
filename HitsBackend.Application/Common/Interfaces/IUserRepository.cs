using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> GetAllWithStatsAsync();
    Task IncrementPostsCountAsync(Guid userId);
}