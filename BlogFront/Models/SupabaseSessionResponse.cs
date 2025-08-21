namespace BlogFront.Models
{
    public class SupabaseUser
    {
        public string? Id { get; set; }
        public string? Email { get; set; }

        // Match Supabase JSON key "email_confirmed_at"
        public DateTime? EmailConfirmedAt { get; set; }
    }

    public class SupabaseSessionResponse
    {
        public SupabaseUser? User { get; set; }
        public string? Access_Token { get; set; }
        public string? Refresh_Token { get; set; }
    }
}
