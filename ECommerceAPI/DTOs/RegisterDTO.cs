using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ECommerceAPI.DTOs
{
    public class RegisterDTO
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string? HomeAddress { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
