using System.ComponentModel.DataAnnotations;

namespace Newsfeed.Models.DTO;

public class PostFileDTO
{
    public string? Data { get; set; }

    public int? StatusCode { get; set; }

    public bool Success { get; set; }
}
