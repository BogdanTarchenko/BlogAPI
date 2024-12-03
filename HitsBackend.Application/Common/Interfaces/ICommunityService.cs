using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface ICommunityService
{
    Task<CommunityFullDto?> GetCommunityByIdAsync(Guid id);
    Task<List<CommunityDto>> GetAllCommunitiesAsync();
    Task CreateCommunityAsync(CommunityDto communityDto);
    Task UpdateCommunityAsync(Guid id, CommunityDto communityDto);
    Task SubscribeAsync(Guid communityId, Guid userId);
    Task UnsubscribeAsync(Guid communityId, Guid userId);
    Task<List<CommunityUserDto>> GetUserCommunitiesAsync(Guid userId);
}