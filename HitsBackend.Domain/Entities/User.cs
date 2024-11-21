using HitsBackend.Domain.Enums;

namespace HitsBackend.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public DateTime CreateTime { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Gender Gender { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? PhoneNumber { get; private set; }
}