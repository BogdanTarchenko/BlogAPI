using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Services;

public class  CommunityService : ICommunityService
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityUserRepository _communityUserRepository;
    private readonly IPostService _postService;
    private readonly ITagRepository _tagRepository;
    private readonly EmailService _emailService;

    public CommunityService(ICommunityRepository communityRepository, ICommunityUserRepository communityUserRepository,
        IPostService postService, ITagRepository tagRepository, EmailService emailService)
    {
        _communityRepository = communityRepository;
        _communityUserRepository = communityUserRepository;
        _postService = postService;
        _tagRepository = tagRepository;
        _emailService = emailService;
    }

    public async Task<CommunityFullDto?> GetCommunityByIdAsync(Guid id)
    {
        var community = await _communityRepository.GetByIdAsync(id);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), id);
        }

        var communityUsers = await _communityUserRepository.GetCommunityUsersAsync(id);

        var administrators = communityUsers
            .Where(cu => cu.Role == CommunityRole.Administrator)
            .Select(cu => new UserDto
            {
                Id = cu.UserId,
                FullName = cu.User.FullName,
                CreateTime = cu.User.CreateTime,
                Gender = cu.User.Gender,
                Email = cu.User.Email,
                BirthDate = cu.User.BirthDate,
                PhoneNumber = cu.User.PhoneNumber
            })
            .ToList();

        return new CommunityFullDto
        {
            Id = community.Id,
            CreateTime = community.CreateTime,
            Name = community.Name,
            Description = community.Description,
            IsClosed = community.IsClosed,
            SubscribersCount = community.SubscribersCount,
            Administrators = administrators
        };
    }

    public async Task<List<CommunityDto>> GetAllCommunitiesAsync()
    {
        var communities = await _communityRepository.GetAllAsync();
        return communities.Select(MapToDto).ToList();
    }

    public async Task SubscribeAsync(Guid communityId, Guid userId)
    {
        var community = await _communityRepository.GetByIdAsync(communityId);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), communityId);
        }

        var isSubscribed = await _communityUserRepository.IsUserSubscribedAsync(communityId, userId);
        if (isSubscribed)
        {
            throw new ValidationException("User is already subscribed to this community.");
        }

        await _communityUserRepository.AddUserToCommunityAsync(communityId, userId);
        community.SubscribersCount++;
        await _communityRepository.UpdateAsync(community);
    }

    public async Task UnsubscribeAsync(Guid communityId, Guid userId)
    {
        var community = await _communityRepository.GetByIdAsync(communityId);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), communityId);
        }

        var communityUser = await _communityUserRepository.GetCommunityUserAsync(communityId, userId);
        if (communityUser == null)
        {
            throw new ValidationException("User is not subscribed to this community.");
        }

        if (communityUser.Role == CommunityRole.Administrator)
        {
            throw new ValidationException("Administrators cannot unsubscribe from the community.");
        }

        await _communityUserRepository.RemoveUserFromCommunityAsync(communityId, userId);
        if (community.SubscribersCount > 0)
        {
            community.SubscribersCount--;
            await _communityRepository.UpdateAsync(community);
        }
    }

    public async Task<List<CommunityUserDto>> GetUserCommunitiesAsync(Guid userId)
    {
        var userCommunities = await _communityUserRepository.GetUserCommunitiesAsync(userId);
        return userCommunities.Select(uc => new CommunityUserDto
        {
            CommunityId = uc.Community.Id,
            Role = uc.Role,
            UserId = userId
        }).ToList();
    }

    public async Task<Guid> CreatePostInCommunityAsync(Guid communityId, Guid userId, CreatePostDto dto)
    {
        var communityUser = await _communityUserRepository.GetCommunityUserAsync(communityId, userId);
        if (communityUser == null)
        {
            throw new ValidationException("User is not a member of the community.");
        }

        if (communityUser.Role != CommunityRole.Administrator)
        {
            throw new ValidationException("Only administrators can create posts in this community.");
        }

        var community = await _communityRepository.GetByIdAsync(communityId);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), communityId);
        }

        var postId = await _postService.CreateAsync(userId, dto, communityId, community.Name);

        var subscribers = await _communityUserRepository.GetCommunityUsersAsync(communityId);
        var subscriberEmails = subscribers.Select(s => s.User.Email).ToList();

        var subject = "New post: " + dto.Title;
        var body = $"<h1>{dto.Title}</h1><p>{dto.Description}</p>";

        foreach (var email in subscriberEmails)
        {
            await _emailService.SendEmailAsync(email, subject, body);
        }

        return postId;
    }

    public async Task<CommunityRole?> GetUserRoleInCommunityAsync(Guid communityId, Guid userId)
    {
        var communityUser = await _communityUserRepository.GetCommunityUserAsync(communityId, userId);
        if (communityUser == null)
        {
            return null;
        }

        return communityUser.Role;
    }

    public async Task<PostPagedListDto> GetPostsByCommunityIdAsync(
        Guid communityId,
        List<Guid>? tags,
        PostSorting sorting,
        int page,
        int size,
        Guid? userId)
    {
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
        
        var community = await _communityRepository.GetByIdAsync(communityId);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), communityId);
        }

        if (community.IsClosed)
        {
            if (!userId.HasValue || !await _communityUserRepository.IsUserSubscribedAsync(communityId, userId.Value))
            {
                throw new ForbiddenException("Access to this community's posts is restricted.");
            }
        }

        return await _postService.GetAllByCommunityIdAsync(communityId, tags, sorting, page, size, userId);
    }

    private static CommunityDto MapToDto(Community community)
    {
        return new CommunityDto
        {
            Id = community.Id,
            CreateTime = community.CreateTime,
            Name = community.Name,
            Description = community.Description,
            IsClosed = community.IsClosed,
            SubscribersCount = community.SubscribersCount
        };
    }
}