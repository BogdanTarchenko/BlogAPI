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
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Likes)
            .AsQueryable();

        if (tags != null && tags.Any())
        {
            query = query.Where(p => p.PostTags.Any(pt => tags.Contains(pt.TagId)));
        }

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(p => p.Author.FullName.ToLower().Contains(author.ToLower()));
        }

        if (min.HasValue)
        {
            query = query.Where(p => p.ReadingTime >= min.Value);
        }

        if (max.HasValue)
        {
            query = query.Where(p => p.ReadingTime <= max.Value);
        }

        query = sorting switch
        {
            PostSorting.CreateAsc => query.OrderBy(p => p.CreateTime),
            PostSorting.CreateDesc => query.OrderByDescending(p => p.CreateTime),
            PostSorting.LikeAsc => query.OrderBy(p => p.Likes.Count),
            PostSorting.LikeDesc => query.OrderByDescending(p => p.Likes.Count),
            _ => query.OrderByDescending(p => p.CreateTime)
        };

        var totalCount = await query.CountAsync();

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

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddLikeAsync(Guid postId, Guid userId)
    {
        var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);
        if (post != null && !post.Likes.Any(l => l.UserId == userId))
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                post.Likes.Add(new PostLike
                {
                    PostId = postId,
                    Post = post,
                    UserId = userId,
                    User = user,
                    CreateTime = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task RemoveLikeAsync(Guid postId, Guid userId)
    {
        var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);
        var like = post?.Likes.FirstOrDefault(l => l.UserId == userId);
        if (like != null)
        {
            post?.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasUserLikedPostAsync(Guid postId, Guid? userId)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == postId);
        return post?.Likes.Any(l => l.UserId == userId) ?? false;
    }

    public async Task<(List<Post> Posts, int TotalCount)> GetAllByCommunityIdAsync(
        Guid communityId,
        List<Guid>? tags = null,
        PostSorting sorting = PostSorting.CreateDesc,
        int page = 1,
        int size = 5)
    {
        var query = _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Likes)
            .Where(p => p.CommunityId == communityId)
            .AsQueryable();

        if (tags != null && tags.Any())
        {
            query = query.Where(p => p.PostTags.Any(pt => tags.Contains(pt.TagId)));
        }

        query = sorting switch
        {
            PostSorting.CreateAsc => query.OrderBy(p => p.CreateTime),
            PostSorting.CreateDesc => query.OrderByDescending(p => p.CreateTime),
            PostSorting.LikeAsc => query.OrderBy(p => p.Likes.Count),
            PostSorting.LikeDesc => query.OrderByDescending(p => p.Likes.Count),
            _ => query.OrderByDescending(p => p.CreateTime)
        };

        var totalCount = await query.CountAsync();
        var posts = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (posts, totalCount);
    }
} 