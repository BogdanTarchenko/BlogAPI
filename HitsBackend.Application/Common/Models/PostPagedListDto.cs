namespace HitsBackend.Application.Common.Models;

public record PostPagedListDto
{
    public List<PostDto>? Posts { get; init; }
    public required PageInfoModel Pagination { get; init; }
}