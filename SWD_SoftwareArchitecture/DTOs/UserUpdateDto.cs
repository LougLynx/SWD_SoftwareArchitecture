using System.ComponentModel.DataAnnotations;

namespace SWD_SoftwareArchitecture.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Range(0, 2, ErrorMessage = "Role must be between 0 and 2")]
        public int Role { get; set; } 
    }
}