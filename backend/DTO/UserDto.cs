using moviesApi.Models;

namespace moviesApi.DTO;

public class UserDto
{
    public Guid PublicId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<UserDto> Following { get; set; }
    public List<UserDto> Followers { get; set; }
}