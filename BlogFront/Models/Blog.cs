namespace BlogFront.Models
{
    public class Blog
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }

        public string Description { get; set; } 
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
