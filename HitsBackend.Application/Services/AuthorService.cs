using HitsBackend.Application.Common.Interfaces;
using HitsBackend.Application.Common.Models;

namespace HitsBackend.Application.Services;

    public class AuthorService : IAuthorService
    {
        private readonly IUserRepository _userRepository;

        public AuthorService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<AuthorDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllWithStatsAsync();
            return users
                .Where(u => u.PostsCount > 0)
                .Select(u => new AuthorDto
                {
                    FullName = u.FullName,
                    BirthDate = u.BirthDate,
                    Gender = u.Gender,
                    Posts = u.PostsCount,
                    Likes = u.LikesCount,
                    Created = u.CreateTime
                })
                .OrderBy(a => a.FullName)
                .ToList();
        }
    }