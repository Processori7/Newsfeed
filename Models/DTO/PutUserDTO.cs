using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Models.DTO;

public class PutUserDTO
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

    public Guid Id { get; }
}
