using moviesApi.DTO;

namespace moviesApi.Services;

public interface IPostRepository
{
    Task<bool> AddPost(PostDto post, string authorId);
    Task<bool> UpdatePost(PostDto post, string authorId);
    Task<bool> DeletePost(PostDto post, string authorId);
    Task<List<PostDto>> Discover(int page, string userId);
    Task<List<PostDto>> Feed(int page, string userId);
    Task<List<PostDto>> GetPostByAuthorId(string authorId, int page);
}