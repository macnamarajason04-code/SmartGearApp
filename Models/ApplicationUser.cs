using Microsoft.AspNetCore.Identity;

namespace SmartGearApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
