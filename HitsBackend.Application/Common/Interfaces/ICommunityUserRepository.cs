using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;
using System.Threading.Tasks;

public interface ICommunityUserRepository
{
    Task<bool> IsUserSubscribedAsync(Guid communityId, Guid userId);
    Task AddUserToCommunityAsync(Guid communityId, Guid userId);
    Task RemoveUserFromCommunityAsync(Guid communityId, Guid userId);
    Task<List<CommunityUser>> GetUserCommunitiesAsync(Guid userId);
    Task<CommunityUser?> GetCommunityUserAsync(Guid communityId, Guid userId);
    Task<List<CommunityUser>> GetCommunityUsersAsync(Guid communityId);
} 