using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;
using HitsBackend.Domain.Enums;

namespace HitsBackend.Application.Services;

public class CommunityService : ICommunityService
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityUserRepository _communityUserRepository;
    private readonly IPostService _postService;

    public CommunityService(ICommunityRepository communityRepository, ICommunityUserRepository communityUserRepository, IPostService postService)
    {
        _communityRepository = communityRepository;
        _communityUserRepository = communityUserRepository;
        _postService = postService;
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

    public async Task CreateCommunityAsync(CommunityDto communityDto)
    {
        var community = MapToEntity(communityDto);
        await _communityRepository.AddAsync(community);
    }

    public async Task UpdateCommunityAsync(Guid id, CommunityDto communityDto)
    {
        var community = await _communityRepository.GetByIdAsync(id);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), id);
        }

        UpdateEntityFromDto(community, communityDto);
        await _communityRepository.UpdateAsync(community);
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

    public async Task<bool> IsUserAdminAsync(Guid communityId, Guid userId)
    {
        var communityUser = await _communityUserRepository.GetCommunityUserAsync(communityId, userId);
        return communityUser != null && communityUser.Role == CommunityRole.Administrator;
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

        return await _postService.CreateAsync(userId, dto);
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

    private static Community MapToEntity(CommunityDto communityDto)
    {
        return new Community
        {
            Id = communityDto.Id,
            CreateTime = communityDto.CreateTime,
            Name = communityDto.Name,
            Description = communityDto.Description,
            IsClosed = communityDto.IsClosed,
            SubscribersCount = communityDto.SubscribersCount
        };
    }

    private static void UpdateEntityFromDto(Community community, CommunityDto communityDto)
    {
        community.Name = communityDto.Name;
        community.Description = communityDto.Description;
        community.IsClosed = communityDto.IsClosed;
        community.SubscribersCount = communityDto.SubscribersCount;
    }
}