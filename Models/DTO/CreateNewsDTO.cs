using System.ComponentModel.DataAnnotations;
using Newsfeed.Models.DTO;

namespace Newsfeed.Models.DTO;

public class CreateNewsDTO
{
    public string Description { get; set; }

    public string Image { get; set; }

    public List<string> Tags { get; set; }

    public string Title { get; set; }
}
