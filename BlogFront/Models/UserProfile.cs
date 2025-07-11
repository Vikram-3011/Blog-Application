using System.ComponentModel.DataAnnotations;

namespace BlogFront.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 120, ErrorMessage = "Please enter a valid age")]
        public int Age { get; set; }

        public string Bio { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
