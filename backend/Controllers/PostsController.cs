using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using moviesApi.DTO;
using moviesApi.Models;
using moviesApi.Services;

namespace moviesApi.Controllers;

[ApiController]
[Route("[controller]")]


public class PostsController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly dbContext _dbContext;
    private readonly IPostRepository _postRepository;

    public PostsController(ILogger<UsersController> logger, IHttpContextAccessor httpContextAccessor, dbContext dbContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _postRepository = new PostRepository(_dbContext);
    }

    [HttpGet]
    [Route("discover")]
    [Authorize]
    public async Task<ActionResult<List<PostDto>>> GetPosts([FromQuery(Name = "page")] int page = 1)
    {
        if (page < 1)
        {
            return BadRequest("Page can't be negative");
        }

        string userId = GetUserId();

        if (String.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        return await _postRepository.Discover(page, userId);
    }

    [HttpGet]
    [Route("feed")]
    [Authorize]
    public async Task<ActionResult<List<PostDto>>> GetPostsFollowing([FromQuery(Name = "page")] int page = 1)
    {
        if (page < 1)
        {
            return BadRequest("Page can't be negative");
        }

        string userId = GetUserId();

        if (String.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        return await _postRepository.Feed(page, userId);
    }

    [HttpPost]
    [Route("add")]
    [Authorize]
    public async Task<IActionResult> AddPost(PostDto post)
    {
        if (post == null)
        {
            return BadRequest();
        }

        string userId = GetUserId();

        if (String.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        bool result = await _postRepository.AddPost(post, userId);

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("getPostByAuthorId")]
    [Authorize]
    public async Task<ActionResult<List<PostDto>>> GetPostsByAuthorId([FromQuery(Name = "authorId")] string authorId, [FromQuery(Name = "page")] int page = 1)
    {
        if (String.IsNullOrEmpty(authorId))
        {
            return BadRequest();
        }

        if (page < 1)
        {
            return BadRequest("Page can't be negative");
        }

        try
        {
            List<PostDto> posts = await _postRepository.GetPostByAuthorId(authorId, page);
            return Ok(posts);

        }
        catch (System.Exception)
        {

            throw;
        }
    }

    [HttpPatch]
    [Route("update")]
    [Authorize]

    public async Task<IActionResult> Update(PostDto post)
    {
        if (post == null)
        {
            return BadRequest();
        }

        string userId = GetUserId();

        if (String.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        bool result = await _postRepository.UpdatePost(post, userId);

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize]

    public async Task<IActionResult> Delete(PostDto post)
    {
        if (post == null)
        {
            return BadRequest();
        }

        string userId = GetUserId();

        if (String.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        bool result = await _postRepository.DeletePost(post, userId);

        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }

    private string GetUserId()
    {
        return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}