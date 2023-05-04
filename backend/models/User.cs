using System.ComponentModel.DataAnnotations;

namespace moviesApi.Models;

public enum Roles
{
    User,
    Admin
}

public class User
{
    [Key]
    public Guid Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public byte[] Salt { get; set; }
    public string Password { get; set; }
    public Roles Role { get; set; } = Roles.User;
    public List<Post> Post { get; set; } = new List<Post>();
    public List<User> Following { get; set; } = new List<User>();
    public List<User> Followers { get; set; } = new List<User>();

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
