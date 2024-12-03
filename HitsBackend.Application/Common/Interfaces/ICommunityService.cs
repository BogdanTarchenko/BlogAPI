using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Common.Interfaces;

public interface ICommunityService
{
    Task<CommunityDto?> GetCommunityByIdAsync(Guid id);
    Task<List<CommunityDto>> GetAllCommunitiesAsync();
    Task CreateCommunityAsync(CommunityDto communityDto);
    Task UpdateCommunityAsync(Guid id, CommunityDto communityDto);
}