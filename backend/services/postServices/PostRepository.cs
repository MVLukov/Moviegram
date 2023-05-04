using System.Globalization;
using Microsoft.EntityFrameworkCore;
using moviesApi.DTO;
using moviesApi.Models;

namespace moviesApi.Services;

public class PostRepository : IPostRepository
{
    private readonly dbContext _dbContext;
    private int itemsPerPage = 4;
    public PostRepository(dbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddPost(PostDto post, string authorId)
    {
        Guid userId = Guid.Parse(authorId);
        User getUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (getUser != null)
        {
            Post newPost = new Post();
            newPost.Author = getUser;
            newPost.Title = post.Title;
            newPost.Description = post.Description;
            newPost.MovieId = post.MovieId;
            newPost.PersonalRating = post.PersonalRating;

            await _dbContext.Posts.AddAsync(newPost);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> UpdatePost(PostDto post, string authorId)
    {
        Guid userId = Guid.Parse(authorId);
        Guid postId = Guid.Parse(post.Id);

        Post getPost = await _dbContext.Posts.Include(x => x.Author).FirstOrDefaultAsync(x => x.Author.Id == userId && x.Id == postId);

        if (getPost != null)
        {
            getPost.Title = post.Title;
            getPost.Description = post.Description;
            getPost.MovieId = post.MovieId;
            getPost.EditedAt = DateTime.UtcNow;
            getPost.PersonalRating = post.PersonalRating;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> DeletePost(PostDto post, string authorId)
    {
        Guid userId = Guid.Parse(authorId);
        Guid postId = Guid.Parse(post.Id);

        Post getPost = await _dbContext.Posts.Include(x => x.Author).FirstOrDefaultAsync(x => x.Author.Id == userId && x.Id == postId);

        if (getPost != null)
        {
            _dbContext.Posts.Remove(getPost);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        return false;
    }

    public async Task<List<PostDto>> GetPostByAuthorId(string authorId, int page)
    {
        Guid userId = Guid.Parse(authorId);
        List<Post> getPosts = new List<Post>();

        if (page == 1)
        {
            getPosts = await _dbContext.Posts.Include(post => post.Author).Where(post => post.Author.PublicId == userId)
                        .OrderByDescending(x => x.PublishedAt).Skip(0).Take(itemsPerPage).AsNoTracking().ToListAsync();
        }
        else
        {
            getPosts = await _dbContext.Posts.Include(post => post.Author).Where(post => post.Author.PublicId == userId)
                        .OrderByDescending(x => x.PublishedAt).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).AsNoTracking().ToListAsync();
        }

        List<PostDto> postsParsed = getPosts.Select(post => ParsePostToDTO(post)).ToList();

        return postsParsed;
    }

    public async Task<List<PostDto>> Discover(int page, string userId)
    {
        Guid userIdParsed = Guid.Parse(userId);
        User getUser = await _dbContext.Users.Include(x => x.Following).FirstOrDefaultAsync(x => x.Id == userIdParsed);
        List<Post> getPosts = new List<Post>();

        if (getUser != null)
        {
            if (page == 1)
            {
                getPosts = await _dbContext.Posts.Include(x => x.Author)
                            .Where(x => !getUser.Following.Contains(x.Author) && x.Author.Id != getUser.Id)
                            .OrderByDescending(x => x.PublishedAt).Skip(0).Take(itemsPerPage).AsNoTracking().ToListAsync();
            }
            else
            {
                getPosts = await _dbContext.Posts.Include(x => x.Author)
                            .Where(x => !getUser.Following.Contains(x.Author) && x.Author.Id != getUser.Id)
                            .OrderByDescending(x => x.PublishedAt).Skip((page - 1) * itemsPerPage).AsNoTracking().Take(itemsPerPage).ToListAsync();
            }
        }

        return getPosts.Select(x => ParsePostToDTO(x)).ToList();
    }

    public async Task<List<PostDto>> Feed(int page, string userId)
    {
        Guid userIdParsed = Guid.Parse(userId);

        User getUser = await _dbContext.Users.Include(x => x.Following).FirstOrDefaultAsync(x => x.Id == userIdParsed);
        List<Post> getPosts = new List<Post>();

        if (getUser != null)
        {
            if (page == 1)
            {
                getPosts = await _dbContext.Posts.Include(x => x.Author).OrderByDescending(x => x.PublishedAt)
                            .Where(x => getUser.Following.Contains(x.Author)).Skip(0).Take(itemsPerPage).AsNoTracking().ToListAsync();
            }
            else
            {
                getPosts = await _dbContext.Posts.Include(x => x.Author).OrderByDescending(x => x.PublishedAt)
                            .Where(x => getUser.Following.Contains(x.Author)).Skip((page - 1) * itemsPerPage).AsNoTracking().Take(itemsPerPage).ToListAsync();
            }
        }

        return getPosts.Select(x => ParsePostToDTO(x)).ToList();
    }

    private PostDto ParsePostToDTO(Post post)
    {
        string publishedAt = post.PublishedAt.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        string editedAt = post.EditedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        return new PostDto() { Id = post.Id.ToString(), Title = post.Title, Description = post.Description, MovieId = post.MovieId, AuthorId = post.Author.PublicId.ToString(), AuthorName = post.Author.Username, PersonalRating = post.PersonalRating, PublishedAt = publishedAt, EditedAt = editedAt };
    }
}