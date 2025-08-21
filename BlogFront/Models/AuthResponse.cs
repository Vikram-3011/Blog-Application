namespace BlogFront.Models
{
    public class AuthResponse
    {
        public string Message { get; set; } = string.Empty;
        public UserResponse? User { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
