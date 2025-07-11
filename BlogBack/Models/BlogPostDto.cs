namespace BlogBack.Models
{
    public class BlogPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty; // ✅ ADD THIS LINE
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
