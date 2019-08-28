using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rgnl_server.Models.Entities;

namespace rgnl_server.Interfaces.Repositories
{
    public interface IUserRepository
    {
        IQueryable<AppUser> GetGovernmentUsers();
        Task<IEnumerable<AppUser>> FollowedByUser(int userId);
        Task<IEnumerable<AppUser>> FollowingTheUser(int userId);
    }
}