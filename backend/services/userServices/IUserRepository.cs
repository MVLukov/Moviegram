using moviesApi.DTO;
using moviesApi.Models;

namespace moviesApi.Services;

public interface IUserRepository
{
    Task<bool> AddUser(User user);
    Task<List<UserDto>> GetAll();
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserById(string id);
    Task<List<UserDto>> GetUserByUsername(string username);
    Task<bool> FollowUser(string userId, string authorId);
    Task<bool> UnfollowUser(string userId, string authorId);
    Task<bool> EmailCheck(string email);
    Task<bool> UsernameCheck(string name);
}