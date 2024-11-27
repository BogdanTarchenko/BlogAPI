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
        int size = 5);
        
    Task<Guid> CreateAsync(Guid userId, CreatePostDto dto);

    Task<PostFullDto> GetByIdAsync(Guid id);
} 