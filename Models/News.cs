using System.ComponentModel.DataAnnotations;

public class NewsModel
{
    public string Description { get; set; }

    [Key]
    public int Id { get; set; }

    public string Image { get; set; }

    public List<string> Tags { get; set; }

    public string Title { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }
}
