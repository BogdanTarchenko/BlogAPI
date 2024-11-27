using HitsBackend.Domain.Entities;
using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Common.Interfaces;

public interface IPostRepository
{
    Task<(List<Post> Posts, int TotalCount)> GetAllAsync(
        List<Guid>? tags = null,
        string? author = null,
        int? min = null,
        int? max = null,
        PostSorting sorting = PostSorting.CreateDesc,
        bool onlyMyCommunities = false,
        int page = 1,
        int size = 5);
        
    Task<Post> CreateAsync(Post post);

    Task<Post?> GetByIdAsync(Guid id);
} 