namespace HitsBackend.Application.Common.Models;

public record CommunityFullDto : CommunityDto
{
    public ICollection<UserDto> Administrators { get; init; } = new List<UserDto>();
}