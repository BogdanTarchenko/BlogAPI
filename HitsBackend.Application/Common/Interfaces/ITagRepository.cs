using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
}