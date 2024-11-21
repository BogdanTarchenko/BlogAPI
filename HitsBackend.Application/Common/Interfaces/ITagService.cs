using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Common.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
}