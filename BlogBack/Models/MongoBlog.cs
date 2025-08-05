using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogBack.Models
{
    public class MongoBlog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string BlogId { get; set; } = string.Empty; // For Supabase GUID
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        [BsonElement("tags")]
        public List<string>? Tags { get; set; } = new(); // ✅ Add this line
    }
}
