using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rgnl_server.Models;
using rgnl_server.Models.Entities;

namespace rgnl_server.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ForNpgsqlUseIdentityColumns();

            builder.Entity<Profile>().ToTable(nameof(Profile));
            builder.Entity<Profile>().HasKey(p => p.ProfileId);
            builder.Entity<Profile>().Property(p => p.ProfileId).ValueGeneratedOnAdd();
            builder.Entity<Profile>().Property(p => p.ShortDescription)
                .HasColumnName(nameof(Profile.ShortDescription))
                .HasColumnType("varchar(300)")
                .IsRequired();
            builder.Entity<Profile>().Property(p => p.LongDescription)
                .HasColumnName(nameof(Profile.LongDescription))
                .HasColumnType("text")
                .IsRequired();
            builder.Entity<Profile>().Property(p => p.ContactInformation)
                .HasColumnName(nameof(Profile.ContactInformation))
                .HasColumnType("text")
                .IsRequired();
            builder.Entity<Profile>().Property(p => p.AppUserId)
                .HasColumnName(nameof(Profile.AppUserId))
                .HasColumnType("int")
                .IsRequired();

            builder.Entity<Relationship>().ToTable(nameof(Relationship));
            builder.Entity<Relationship>().HasKey(r => r.RelationshipId);
            builder.Entity<Relationship>().Property(r => r.RelationshipId).ValueGeneratedOnAdd();
            builder.Entity<Relationship>().Property(r => r.FollowerId)
                .HasColumnName(nameof(Relationship.FollowerId))
                .HasColumnType("int")
                .IsRequired();
            builder.Entity<Relationship>().Property(r => r.FolloweeId)
                .HasColumnName(nameof(Relationship.FolloweeId))
                .HasColumnType("int")
                .IsRequired();

            builder.Entity<AppUser>()
                .HasOne(appUser => appUser.Profile)
                .WithOne(profile => profile.AppUser)
                .HasForeignKey<Profile>(profile => profile.AppUserId);
            builder.Entity<AppUser>()
                .HasMany(appUser => appUser.Following)
                .WithOne(relationship => relationship.Follower)
                .HasForeignKey(relationship => relationship.FollowerId);
            builder.Entity<AppUser>()
                .HasMany(appUser => appUser.Followers)
                .WithOne(relationship => relationship.Followee)
                .HasForeignKey(relationship => relationship.FolloweeId);
        }

        public IQueryable<T> AsNoTracking<T>() where T : class
        {
            return this.Set<T>().AsNoTracking();
        }

        public IQueryable<T> AsTracking<T>() where T : class
        {
            return this.Set<T>();
        }
    }
}
