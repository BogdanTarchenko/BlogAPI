using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Interfaces;

public interface IPostService
{
    Task<PostPagedListDto> GetAllAsync(
        List<Guid>? tags = null,
        string? author = null,
        int? min = null,
        int? max = null,
        PostSorting sorting = PostSorting.CreateDesc,
        bool onlyMyCommunities = false,
        int page = 1,
        int size = 5,
        Guid? userId = null);
        
    Task<Guid> CreateAsync(Guid userId, CreatePostDto dto, Guid? communityId = null, string? communityName = null);
    Task<PostFullDto> GetByIdAsync(Guid id, Guid? userId);
    Task AddLikeAsync(Guid postId, Guid userId);
    Task RemoveLikeAsync(Guid postId, Guid userId);
} 