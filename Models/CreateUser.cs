using System.ComponentModel.DataAnnotations;
using Newsfeed.Models.DTO;

public class RegisterUser
{
    [Key]

    public Guid Id { get; set; }

    public string? Avatar { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string Password { get; set; }

    public string? Role { get; set; }

    public string Token { get; set; }
}

public class AuthUser
{
    public string? Avatar { get; set; }

    [MinLength(3)]
    [MaxLength(100)]
    public string? Email { get; set; }

    public int Id { get; set; }

    [MinLength(3)]
    [MaxLength(25)]
    public string? Name { get; set; }

    public string? Password { get; set; }

    [MinLength(3)]
    [MaxLength(25)]
    public string? Role { get; set; }
}
