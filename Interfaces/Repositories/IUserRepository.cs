using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rgnl_server.Models.Entities;

namespace rgnl_server.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser> GetUser(int userId);
        Task<IEnumerable<Post>> GetPosts(int userId);
        IQueryable<AppUser> GetGovernmentUsers();
        Task<IEnumerable<AppUser>> FollowedByUser(int userId);
        Task<IEnumerable<AppUser>> FollowingTheUser(int userId);
        Task<AppUser> UpdateProfile(Profile newProfile);
    }
}