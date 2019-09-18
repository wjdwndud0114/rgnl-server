using rgnl_server.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rgnl_server.Models.Entities;

namespace rgnl_server.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
                .ForNpgsqlUseIdentityColumns()
                .BuildProfileTable()
                .BuildRelationshipTable()
                .BuildPostTable()
                .BuildTableRelationships();
        }

        public IQueryable<T> NonTrackingSet<T>() where T : class
        {
            return this.Set<T>().AsNoTracking();
        }

        public DbSet<T> TrackingSet<T>() where T : class
        {
            return this.Set<T>();
        }
    }
}
