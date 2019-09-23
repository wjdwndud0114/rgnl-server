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
        {
        }

        public Task<AppUser> GetUser(int userId)
        {
            return DbContext.NonTrackingSet<AppUser>()
                .Include(appUser => appUser.Roles)
                    .ThenInclude(e => e.Role)
                .Include(appUser => appUser.Profile)
                .FirstOrDefaultAsync(appUser => appUser.Id == userId);
        }

        public IQueryable<AppUser> GetGovernmentUsers()
        {
            return DbContext.NonTrackingSet<AppUser>()
                .Include(appUser => appUser.Roles)
                    .ThenInclude(e => e.Role)
                .Include(appUser => appUser.Profile)
                .Where(appUser => appUser.Roles.Any(role => role.Role.Name.Equals(Constants.Strings.Roles.Producer)));
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

        public async Task<AppUser> UpdateProfile(Profile delta)
        {
            if (delta.ProfileId == 0)
            {
                await DbContext.AddAsync(new Profile
                {
                    AppUserId = delta.AppUserId,
                    ShortDescription = delta.ShortDescription,
                    LongDescription = delta.LongDescription,
                    Tags = delta.Tags,
                    Url = delta.Url,
                    Street = delta.Street,
                    City = delta.City,
                    State = delta.State,
                    Zip = delta.Zip
                });
            }
            else
            {
                var profile = delta;
                DbContext.Update(profile);
            }

            await DbContext.SaveChangesAsync();
            return await GetUser(delta.AppUserId);
        }
    }
}