using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface ICommunityRepository
{
    Task<Community?> GetByIdAsync(Guid id);
    Task<List<Community>> GetAllAsync();
    Task AddAsync(Community community);
    Task UpdateAsync(Community community);
}