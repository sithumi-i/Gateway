using Microsoft.AspNetCore.Identity;

namespace Gateway.BlindMatch.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
