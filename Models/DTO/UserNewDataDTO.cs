using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Models.DTO;

public class UserNewDataDTO
{
    public string? Avatar { get; set; }

    [MinLength(3)]
    [MaxLength(100)]
    public string? Email { get; set; }

    [MinLength(3)]
    [MaxLength(25)]
    public string? Name { get; set; }

    [MinLength(3)]
    [MaxLength(25)]
    public string? Role { get; set; }

    public int Id { get; }

    public string Token { get; }
}
