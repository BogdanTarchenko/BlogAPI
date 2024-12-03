using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Enums;

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
    Task<bool> IsUserAdminAsync(Guid communityId, Guid userId);
    Task<Guid> CreatePostInCommunityAsync(Guid communityId, Guid userId, CreatePostDto dto);
    Task<PostPagedListDto> GetPostsByCommunityIdAsync(
        Guid communityId,
        List<Guid>? tags,
        PostSorting sorting,
        int page,
        int size,
        Guid? userId);
    Task<CommunityRole?> GetUserRoleInCommunityAsync(Guid communityId, Guid userId);
}