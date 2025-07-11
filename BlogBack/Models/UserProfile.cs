using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace BlogBack.Models
{
    [Table("Userprofile")]
    public class UserProfile : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("age")]
        public int Age { get; set; }

        [Column("bio")]
        public string Bio { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
