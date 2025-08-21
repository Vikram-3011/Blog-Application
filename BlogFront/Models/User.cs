namespace BlogFront.Models
{
    public class User
    {
        public string Email { get; set; }
        public string? Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSuperAdmin { get; set; }

        public bool EmailConfirmed { get; set; }
        public User(string email, string? name = null, bool emailConfirmed = false)
        {
            Email = email;
            Name = name;
            this.EmailConfirmed = emailConfirmed;
        }

    }
}
