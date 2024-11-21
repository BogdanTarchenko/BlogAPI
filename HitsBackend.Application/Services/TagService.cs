using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<TagDto>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            CreateTime = t.CreateTime
        }).ToList();
    }
}