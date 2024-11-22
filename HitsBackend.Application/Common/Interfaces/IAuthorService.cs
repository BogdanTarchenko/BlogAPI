using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Common.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
} 