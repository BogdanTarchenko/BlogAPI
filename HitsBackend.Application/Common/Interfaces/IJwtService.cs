using HitsBackend.Domain.Entities;

namespace HitsBackend.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}