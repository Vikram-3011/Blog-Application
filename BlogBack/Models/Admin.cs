using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace BlogBack.Models
{
    [Table("admins")]
    public class Admin : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("is_super")]
        public bool IsSuper { get; set; }
    }
}
