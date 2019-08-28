using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rgnl_server.Data;
using rgnl_server.Helpers;
using rgnl_server.Interfaces.Repositories;
using rgnl_server.Models.Entities;

namespace rgnl_server.Repositories
{
    public class UserRepository : AppRepository, IUserRepository
    {

        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        { }

        public IQueryable<AppUser> GetGovernmentUsers()
        {
            return DbContext.NonTrackingSet<AppUser>()
                .Include(user => user.Roles)
                .Include(user => user.Profile)
                .Where(user => user.Roles.Any(role => role.Name.Equals(Constants.Strings.Roles.Producer)));
        }

        public async Task<IEnumerable<AppUser>> FollowedByUser(int userId)
        {
            var user = await DbContext.NonTrackingSet<AppUser>()
                .Include(appUser => appUser.Following)
                    .ThenInclude(relationship => relationship.Followee)
                        .ThenInclude(appUser => appUser.Profile)
                .FirstOrDefaultAsync(appUser => appUser.Id == userId);

            return user.Following
                .Select(relationship => relationship.Followee);
        }

        public async Task<IEnumerable<AppUser>> FollowingTheUser(int userId)
        {
            var user = await DbContext.NonTrackingSet<AppUser>()
                .Include(appUser => appUser.Followers)
                    .ThenInclude(relationship => relationship.Follower)
                .FirstOrDefaultAsync(appUser => appUser.Id == userId);

            return user.Followers
                .Select(relationship => relationship.Follower);
        }
    }
}