
using BlogBack.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace BlogBack.Models
{
    [Table("Blogapp")] // ✅ Match Supabase table name exactly
    public class BlogPost : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("author_email")]
        public string AuthorEmail { get; set; } = string.Empty;

            
        [Column("author")]
        public string Author { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("tags")]
        public List<string> Tags { get; set; } = new(); // ✅ ADD THIS
    }
}
