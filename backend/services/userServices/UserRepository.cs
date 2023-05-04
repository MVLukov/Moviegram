using Microsoft.EntityFrameworkCore;
using moviesApi.DTO;
using moviesApi.Models;

namespace moviesApi.Services;

public class UserRepository : IUserRepository
{
    private readonly dbContext _dbContext;
    public UserRepository(dbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddUser(User user)
    {
        try
        {
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
        catch (System.Exception)
        {

            return false;
        }

        return true;
    }


    public async Task<List<UserDto>> GetAll()
    {
        List<User> getUsers = await _dbContext.Users.AsNoTracking().ToListAsync();
        return getUsers.Select(x => UserParsed(x)).ToList();
    }


    public async Task<bool> UsernameCheck(string username)
    {
        return await _dbContext.Users.AnyAsync(x => x.Username == username);
    }
    public async Task<bool> EmailCheck(string email)
    {
        return await _dbContext.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<User> GetUserById(string id)
    {
        return await _dbContext.Users.Include(x => x.Followers).Include(x => x.Following).AsNoTracking().FirstOrDefaultAsync(x => x.PublicId == Guid.Parse(id));
    }
    public async Task<User> GetUserByEmail(string email)
    {
        return await _dbContext.Users.Include(x => x.Followers).Include(x => x.Following).FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> FollowUser(string userId, string userIdToFollow)
    {
        Guid userIdParsed = Guid.Parse(userId);
        Guid userIdToFollowParsed = Guid.Parse(userIdToFollow);

        User getUser = await _dbContext.Users.Include(x => x.Followers).Include(x => x.Following).FirstOrDefaultAsync(x => x.Id == userIdParsed);
        User getUserToFollow = await _dbContext.Users.Include(x => x.Followers).Include(x => x.Following).FirstOrDefaultAsync(x => x.PublicId == userIdToFollowParsed);

        if (userIdParsed == userIdToFollowParsed)
        {
            return false;
        }

        if (getUser != null && getUserToFollow != null)
        {
            if (getUser.Following.Any(x => x.Id == getUserToFollow.Id))
            {
                return false;
            }

            getUser.Following.Add(getUserToFollow);
            getUserToFollow.Followers.Add(getUser);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> UnfollowUser(string userId, string userIdToUnfollow)
    {
        Guid userIdParsed = Guid.Parse(userId);
        Guid userIdToUnfollowParsed = Guid.Parse(userIdToUnfollow);

        User getUser = await _dbContext.Users.Include(x => x.Followers).Include(x => x.Following).FirstOrDefaultAsync(x => x.Id == userIdParsed);
        User getUserToUnfollow = await _dbContext.Users.Include(x => x.Followers).Include(x => x.Following).FirstOrDefaultAsync(x => x.PublicId == userIdToUnfollowParsed);

        if (userIdParsed == userIdToUnfollowParsed)
        {
            return false;
        }

        if (getUser != null && getUserToUnfollow != null)
        {
            if (getUser.Following.Any(x => x.Id == getUserToUnfollow.Id))
            {
                getUser.Following.Remove(getUserToUnfollow);
                getUserToUnfollow.Followers.Remove(getUser);

                await _dbContext.SaveChangesAsync();
            }
            return true;
        }
        return false;
    }

    public async Task<List<UserDto>> GetUserByUsername(string username)
    {
        List<User> getUsers = await _dbContext.Users.Where(x => x.Username.ToLower().StartsWith(username.ToLower())).ToListAsync();

        return getUsers.Select(x => new UserDto() { PublicId = x.PublicId, Username = x.Username }).ToList();
    }

    private UserDto UserParsed(User x)
    {
        return new UserDto()
        {
            PublicId = x.PublicId,
            Username = x.Username,
            Email = x.Email,
            RoleId = (int)x.Role,
            RoleName = ((Roles)x.Role).ToString()
        };
    }
}