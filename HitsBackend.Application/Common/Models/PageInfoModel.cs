namespace HitsBackend.Application.Common.Models;

public record PageInfoModel
{
    public int Size { get; init; }
    public int Count { get; init; }
    public int Current { get; init; }
}