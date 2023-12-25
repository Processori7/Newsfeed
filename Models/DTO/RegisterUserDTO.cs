using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Models.DTO;

public class RegisterUserDTO
{
    public string? Avatar { get; set; }

    [MinLength(3)]
    [MaxLength(100)]
    public string? Email { get; set; }

    [MinLength(3)]
    [MaxLength(25)]
    public string? Name { get; set; }

    public string? Password { get; set; }

    [MinLength(3)]
    [MaxLength(25)]
    public string? Role { get; set; }

    public Guid Id { get; }

    public string Token { get; set; }
}
