using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string HomeAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
