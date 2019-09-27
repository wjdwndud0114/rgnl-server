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

        public async Task<IEnumerable<Post>> GetPosts(int userId)
        {
            var userIds = DbContext.NonTrackingSet<Relationship>()
                .Include(r => r.Followee)
                .ThenInclude(u => u.Profile)
                .Where(r => r.FollowerId == userId)
                .Select(r => r.Followee.Id);

            var posts = await DbContext.NonTrackingSet<Post>()
                .Include(p => p.AppUser)
                    .ThenInclude(u => u.Profile)
                .OrderByDescending(p => p.PostId)
                .Where(p => userId == p.AppUserId || userIds.Contains(p.AppUserId))
                .ToListAsync();

            posts.ForEach(p => p.AppUser.Posts = null);
            return posts;
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
            var followedUsers = DbContext.NonTrackingSet<Relationship>()
                .Include(r => r.Followee)
                .ThenInclude(u => u.Profile)
                .Where(r => r.FollowerId == userId)
                .Select(r => r.Followee);

            await followedUsers.ForEachAsync(u => u.Followers = null);
            return followedUsers;
        }

        public async Task<IEnumerable<AppUser>> FollowingTheUser(int userId)
        {
            var followingUsers = DbContext.NonTrackingSet<Relationship>()
                .Include(r => r.Follower)
                .ThenInclude(u => u.Profile)
                .Where(r => r.FolloweeId == userId)
                .Select(r => r.Follower);

            await followingUsers.ForEachAsync(u => u.Following = null);
            return followingUsers;
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

        public async Task<bool> FollowUser(int govId, int userId)
        {
            var role = await DbContext.NonTrackingSet<AppUserRole>()
                .FirstOrDefaultAsync(u => u.UserId == govId && u.Role.Name.Equals(Constants.Strings.Roles.Producer));

            if (role == null)
            {
                return false;
            }

            await DbContext.AddAsync(new Relationship
            {
                FolloweeId = govId,
                FollowerId = userId
            });
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnfollowUser(int govId, int userId)
        {
            var relationship = await DbContext.TrackingSet<Relationship>()
                .FirstOrDefaultAsync(u => u.FolloweeId == govId && u.FollowerId == userId);

            if (relationship == null)
            {
                return false;
            }

            DbContext.Remove(relationship);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}