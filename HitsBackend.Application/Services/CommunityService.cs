using HitsBackend.Application.Common.Exceptions;
using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;
using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Services;

public class CommunityService : ICommunityService
{
    private readonly ICommunityRepository _communityRepository;

    public CommunityService(ICommunityRepository communityRepository)
    {
        _communityRepository = communityRepository;
    }

    public async Task<CommunityDto?> GetCommunityByIdAsync(Guid id)
    {
        var community = await _communityRepository.GetByIdAsync(id);
        if (community == null)
        {
            throw new NotFoundException(nameof(Community), id);
        }
        return MapToDto(community);
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