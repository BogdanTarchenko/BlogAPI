namespace HitsBackend.Application.Common.Interfaces;

public interface ICommunityUserRepository
{
    Task<bool> IsUserSubscribedAsync(Guid communityId, Guid userId);
    Task AddUserToCommunityAsync(Guid communityId, Guid userId);
    Task RemoveUserFromCommunityAsync(Guid communityId, Guid userId);
} 