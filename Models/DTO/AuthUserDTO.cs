using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Models.DTO;

public class AuthUserDTO
{
    [MinLength(3)]
    [MaxLength(100)]
    public string? Email { get; set; }

    public string? Password { get; set; }
}
