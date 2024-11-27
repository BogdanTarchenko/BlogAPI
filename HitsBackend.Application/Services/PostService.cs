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
    
    private readonly IValidator<CreatePostDto> _postValidator;

    public PostService(
        IPostRepository postRepository,
        ITagRepository tagRepository,
        IValidator<CreatePostDto> postValidator,
        IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _postValidator = postValidator;
    }

    public async Task<PostPagedListDto> GetAllAsync(
        List<Guid>? tags = null,
        string? author = null,
        int? min = null,
        int? max = null,
        PostSorting sorting = PostSorting.CreateDesc,
        bool onlyMyCommunities = false,
        int page = 1,
        int size = 5)
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

        var postDtos = posts.Select(p => new PostDto(
            Id: p.Id,
            CreateTime: p.CreateTime,
            Title: p.Title,
            Description: p.Description,
            ReadingTime: p.ReadingTime,
            Image: p.Image,
            AuthorId: p.AuthorId,
            Author: p.Author.FullName,
            CommunityId: p.CommunityId,
            CommunityName: p.CommunityName,
            AddressId: p.AddressId,
            Likes: p.Likes.Count,
            HasLike: false, // TODO: Реализовать проверку лайка для авторизованного пользователя
            CommentsCount: 0, // TODO: Реализовать подсчет комментариев
            Tags: p.PostTags.Select(pt => new TagDto
            {
                Id = pt.Tag.Id,
                Name = pt.Tag.Name,
                CreateTime = pt.Tag.CreateTime
            }).ToList()
        )).ToList();

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

    public async Task<Guid> CreateAsync(Guid userId, CreatePostDto dto)
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
} 