namespace BlogFront.Models
{
    public class SupabaseSessionResponse
    {
        public string? Access_Token { get; set; }
        public SupabaseUser? User { get; set; }
    }

    public class SupabaseUser
    {
        public string? Id { get; set; } 
        public string? Email { get; set; }
    }
}
