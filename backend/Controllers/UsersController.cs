using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using moviesApi.DTO;
using moviesApi.Models;
using moviesApi.Services;
using static moviesApi.Services.GenJwtToken;

namespace moviesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly dbContext _dbContext;

    public UsersController(ILogger<UsersController> logger, IHttpContextAccessor httpContextAccessor, dbContext dbContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _userRepository = new UserRepository(_dbContext);
    }

    [HttpGet]
    [Route("getAll")]
    [Authorize(Roles = "Admin")]
    public async Task<List<UserDto>> GetAllUsers()
    {
        return await _userRepository.GetAll();
    }

    [HttpGet]
    [Route("getUsersByUsername")]
    [Authorize]
    public async Task<ActionResult<List<UserDto>>> GetUsersByUsername([FromQuery(Name = "username")] string username)
    {
        if (String.IsNullOrEmpty(username))
        {
            return BadRequest();
        }

        return await _userRepository.GetUserByUsername(username);
    }

    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> AddUser(SignupDTO user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        bool checkUsername = await _userRepository.UsernameCheck(user.Username);
        bool checkEmail = await _userRepository.EmailCheck(user.Email);

        if (checkUsername && checkEmail)
        {
            return Ok(new { error = "email/username" });
        }

        if (checkUsername)
        {
            return Ok(new { error = "username" });

        }

        if (checkEmail)
        {
            return Ok(new { error = "email" });
        }

        var hash = passwordUtils.HashPassword(user.Password, out var salt);

        User newUser = new User() { Username = user.Username, Email = user.Email, Password = hash, Salt = salt };
        bool result = await _userRepository.AddUser(newUser);

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> Signin(SigninDTO user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        User getUser = await _userRepository.GetUserByEmail(user.Email);

        if (getUser != null)
        {
            bool verifyPass = passwordUtils.VerifyPassword(user.Password, getUser.Password, getUser.Salt);

            if (verifyPass)
            {
                string token = GenJwtToken.CreateToken(getUser);
                Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.Now.AddMinutes(30) });

                List<UserDto> following = getUser.Following.Select(x => new UserDto() { PublicId = x.PublicId, Username = x.Username }).ToList();
                List<UserDto> followers = getUser.Followers.Select(x => new UserDto() { PublicId = x.PublicId, Username = x.Username }).ToList();

                UserDto userPassed = new UserDto() { PublicId = getUser.PublicId, Username = getUser.Username, Email = getUser.Email, RoleId = (int)getUser.Role, RoleName = ((Roles)getUser.Role).ToString() };
                userPassed.Followers = followers;
                userPassed.Following = following;

                return Ok(userPassed);
            }
            else
            {
                return Ok(new { error = "invalid credentials" });
            }
        }

        return Ok(new { error = "not_found" });
    }

    [HttpGet]
    [Route("getUserById")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUserById([FromQuery(Name = "userId")] string userId)
    {
        if (String.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        User getUser = await _userRepository.GetUserById(userId);

        List<UserDto> following = getUser.Following.Select(x => new UserDto() { PublicId = x.PublicId, Username = x.Username }).ToList();
        List<UserDto> followers = getUser.Followers.Select(x => new UserDto() { PublicId = x.PublicId, Username = x.Username }).ToList();

        return Ok(new UserDto() { PublicId = getUser.PublicId, Username = getUser.Username, Followers = followers, Following = following });
    }

    [HttpGet]
    [Route("follow")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Follow([FromQuery(Name = "userIdToFollow")] string userIdToFollow)
    {
        if (String.IsNullOrEmpty(userIdToFollow))
        {
            return BadRequest();
        }

        string id = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (String.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        bool result = await _userRepository.FollowUser(id, userIdToFollow);

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("unfollow")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Unfollow([FromQuery(Name = "userIdToUnfollow")] string userIdToUnfollow)
    {
        if (String.IsNullOrEmpty(userIdToUnfollow))
        {
            return BadRequest();
        }

        string id = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (String.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        bool result = await _userRepository.UnfollowUser(id, userIdToUnfollow);

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("auth")]
    [Authorize]
    public IActionResult Auth()
    {
        return Ok();
    }

    [HttpGet]
    [Route("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("X-Access-Token");
        return Ok();
    }
}

