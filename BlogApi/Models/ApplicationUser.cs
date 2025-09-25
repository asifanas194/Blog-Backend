using Microsoft.AspNetCore.Identity;

namespace BlogApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
