using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface ICommunityRepository
{
    Task<Community?> GetByIdAsync(Guid id);
    Task<List<Community>> GetAllAsync();
    Task UpdateAsync(Community community);
}