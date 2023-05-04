using moviesApi.Models;

namespace moviesApi.DTO;

public class PostDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string MovieId { get; set; }
    public int PersonalRating { get; set; }
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string PublishedAt { get; set; }
    public string EditedAt { get; set; }
}