using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace rgnl_server.Models.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? FacebookId { get; set; }
        public string PictureUrl { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual ICollection<Relationship> Followers { get; set; }
        public virtual ICollection<Relationship> Following { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<AppUserRole> Roles { get; set; }
    }
}