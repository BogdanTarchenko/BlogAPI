using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Domain.Entities;
using HitsBackend.Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Post> Posts, int TotalCount)> GetAllAsync(
        List<Guid>? tags = null,
        string? author = null,
        int? min = null,
        int? max = null,
        PostSorting sorting = PostSorting.CreateDesc,
        bool onlyMyCommunities = false,
        int page = 1,
        int size = 5)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Likes)
            .AsQueryable();

        // tag
        if (tags != null && tags.Any())
        {
            query = query.Where(p => p.PostTags.Any(pt => tags.Contains(pt.TagId)));
        }

        // author
        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(p => p.Author.FullName.ToLower().Contains(author.ToLower()));
        }

        // read time
        if (min.HasValue)
        {
            query = query.Where(p => p.ReadingTime >= min.Value);
        }
        if (max.HasValue)
        {
            query = query.Where(p => p.ReadingTime <= max.Value);
        }

        // sort
        query = sorting switch
        {
            PostSorting.CreateAsc => query.OrderBy(p => p.CreateTime),
            PostSorting.CreateDesc => query.OrderByDescending(p => p.CreateTime),
            PostSorting.LikeAsc => query.OrderBy(p => p.Likes.Count),
            PostSorting.LikeDesc => query.OrderByDescending(p => p.Likes.Count),
            _ => query.OrderByDescending(p => p.CreateTime)
        };
        
        var totalCount = await query.CountAsync();

        // pagination
        var posts = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (posts, totalCount);
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }
} 