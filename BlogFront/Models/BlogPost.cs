using System.ComponentModel.DataAnnotations;

public class BlogPost
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string AuthorEmail { get; set; } = string.Empty;

 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
