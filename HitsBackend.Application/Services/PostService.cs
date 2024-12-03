using FluentValidation;
using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using HitsBackend.Domain.Enums;
using ValidationException = HitsBackend.Application.Common.Exceptions.ValidationException;

namespace HitsBackend.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ICommunityUserRepository _communityUserRepository;
    private readonly ICommunityRepository _communityRepository;
    
    private readonly IValidator<CreatePostDto> _postValidator;

    public PostService(
        IPostRepository postRepository,
        ITagRepository tagRepository,
        IValidator<CreatePostDto> postValidator,
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        ICommunityUserRepository communityUserRepository,
        ICommunityRepository communityRepository)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _postValidator = postValidator;
        _commentRepository = commentRepository;
        _communityUserRepository = communityUserRepository;
        _communityRepository = communityRepository;
    }

    public async Task<PostPagedListDto> GetAllAsync(
        List<Guid>? tags = null,
        string? author = null,
        int? min = null,
        int? max = null,
        PostSorting sorting = PostSorting.CreateDesc,
        bool onlyMyCommunities = false,
        int page = 1,
        int size = 5,
        Guid? userId = null)
    {
        if (author != null && author.Length > 1000)
        {
            throw new ValidationException("Author search string must not exceed 1000 characters");
        }

        if (min.HasValue && min.Value < 0)
        {
            throw new ValidationException("Minimum reading time must be greater than or equal to 0");
        }

        if (max.HasValue && min.HasValue && max.Value < min.Value)
        {
            throw new ValidationException("Maximum reading time must be greater than or equal to minimum reading time");
        }

        if (page <= 0)
        {
            throw new ValidationException("Page number must be greater than 0");
        }

        if (size <= 0)
        {
            throw new ValidationException("Size must be greater than 0");
        }
        
        if (tags != null && tags.Any())
        {
            var existingTags = await _tagRepository.GetAllAsync();
            var validTagIds = existingTags.Select(t => t.Id).ToList();
            var invalidTags = tags.Where(t => !validTagIds.Contains(t)).ToList();
            
            if (invalidTags.Any())
            {
                throw new ValidationException($"Tags with IDs {string.Join(", ", invalidTags)} do not exist");
            }
        }

        var (posts, totalCount) = await _postRepository.GetAllAsync(
            tags, author, min, max, sorting, onlyMyCommunities, page, size);

        var postDtos = new List<PostDto>();

        foreach (var post in posts)
        {
            if (post.CommunityId.HasValue)
            {
                var community = await _communityRepository.GetByIdAsync(post.CommunityId.Value);
                if (community?.IsClosed == true)
                {
                    if (!userId.HasValue || !await _communityUserRepository.IsUserSubscribedAsync(post.CommunityId.Value, userId.Value))
                    {
                        continue;
                    }
                }
            }

            bool hasLike = userId.HasValue && await _postRepository.HasUserLikedPostAsync(post.Id, userId);
            var comments = await _commentRepository.GetByPostIdAsync(post.Id);
            postDtos.Add(new PostDto(
                Id: post.Id,
                CreateTime: post.CreateTime,
                Title: post.Title,
                Description: post.Description,
                ReadingTime: post.ReadingTime,
                Image: post.Image,
                AuthorId: post.AuthorId,
                Author: post.Author.FullName,
                CommunityId: post.CommunityId,
                CommunityName: post.CommunityName,
                AddressId: post.AddressId,
                Likes: post.Likes.Count,
                HasLike: hasLike,
                CommentsCount: comments.Count,
                Tags: post.PostTags.Select(pt => new TagDto
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name,
                    CreateTime = pt.Tag.CreateTime
                }).ToList()
            ));
        }

        return new PostPagedListDto
        {
            Posts = postDtos,
            Pagination = new PageInfoModel
            {
                Size = size,
                Count = totalCount,
                Current = page
            }
        };
    }

    public async Task<Guid> CreateAsync(Guid userId, CreatePostDto dto, Guid? communityId, string? communityName)
    {
        var validationResult = await _postValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var user = await _userRepository.GetByIdAsync(userId) 
            ?? throw new NotFoundException(nameof(User), userId);

        var existingTags = await _tagRepository.GetAllAsync();
        var validTagIds = existingTags.Select(t => t.Id).ToList();
        if (dto.Tags.Any(t => !validTagIds.Contains(t)))
            throw new ValidationException("One or more tags do not exist");

        var postId = Guid.NewGuid();
        var post = new Post
        {
            Id = postId,
            Title = dto.Title,
            Description = dto.Description,
            ReadingTime = dto.ReadingTime,
            Image = dto.Image,
            CreateTime = DateTime.UtcNow,
            AuthorId = userId,
            Author = user,
            AddressId = dto.AddressId,
            CommunityId = communityId,
            CommunityName = communityName,
            PostTags = new List<PostTag>()
        };

        foreach (var tagId in dto.Tags)
        {
            var tag = existingTags.First(t => t.Id == tagId);
            post.PostTags.Add(new PostTag
            {
                PostId = postId,
                Post = post,
                TagId = tagId,
                Tag = tag
            });
        }

        await _postRepository.CreateAsync(post);
        await _userRepository.IncrementPostsCountAsync(userId);
        return post.Id;
    }

    public async Task<PostFullDto> GetByIdAsync(Guid id, Guid? userId)
    {
        var post = await _postRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Post), id);
        
        bool hasLike = userId.HasValue && await _postRepository.HasUserLikedPostAsync(id, userId);
        var comments = await _commentRepository.GetByPostIdAsync(id);
        
        return new PostFullDto(
            Id: post.Id,
            CreateTime: post.CreateTime,
            Title: post.Title,
            Description: post.Description,
            ReadingTime: post.ReadingTime,
            Image: post.Image,
            AuthorId: post.AuthorId,
            Author: post.Author.FullName,
            CommunityId: post.CommunityId,
            CommunityName: post.CommunityName,
            AddressId: post.AddressId,
            Likes: post.Likes.Count,
            HasLike: hasLike,
            CommentsCount: comments.Count,
            Tags: post.PostTags.Select(pt => new TagDto
            {
                Id = pt.Tag.Id,
                Name = pt.Tag.Name,
                CreateTime = pt.Tag.CreateTime
            }).ToList(),
            Comments: comments.Select(c => new CommentDto(
                Id: c.Id,
                CreateTime: c.CreateTime,
                Content: c.Content,
                ModifiedDate: c.ModifiedDate,
                DeleteDate: c.DeleteDate,
                AuthorId: c.AuthorId,
                Author: c.Author.FullName,
                SubComments: c.Replies.Count
            )).ToList()
        );
    }

    public async Task AddLikeAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException(nameof(Post), postId);
        }
        if (await _postRepository.HasUserLikedPostAsync(postId, userId))
        {
            throw new ValidationException("User has already liked this post.");
        }

        await _postRepository.AddLikeAsync(postId, userId);
        await _userRepository.IncrementLikesCountAsync(post.AuthorId);
    }

    public async Task RemoveLikeAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException(nameof(Post), postId);
        }

        if (!await _postRepository.HasUserLikedPostAsync(postId, userId))
        {
            throw new ValidationException("User hasn't liked this post.");
        }

        await _postRepository.RemoveLikeAsync(postId, userId);
        await _userRepository.DecrementLikesCountAsync(post.AuthorId);
    }
} 