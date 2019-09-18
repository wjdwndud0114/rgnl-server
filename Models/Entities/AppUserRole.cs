using Microsoft.AspNetCore.Identity;

namespace rgnl_server.Models.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public virtual IdentityRole<int> Role { get; set; }
    }
}