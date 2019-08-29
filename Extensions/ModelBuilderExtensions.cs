using Microsoft.EntityFrameworkCore;
using rgnl_server.Models.Entities;

namespace rgnl_server.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder BuildProfileTable(this ModelBuilder builder)
        {
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
            builder.Entity<Profile>().Property(p => p.Tags)
                .HasColumnName(nameof(Profile.Tags))
                .HasColumnType("text")
                .IsRequired(false);
            builder.Entity<Profile>().Property(p => p.Url)
                .HasColumnName(nameof(Profile.Url))
                .HasColumnType("text")
                .IsRequired(false);
            builder.Entity<Profile>().Property(p => p.Address)
                .HasColumnName(nameof(Profile.Address))
                .HasColumnType("text")
                .IsRequired(true);
            builder.Entity<Profile>().Property(p => p.AppUserId)
                .HasColumnName(nameof(Profile.AppUserId))
                .HasColumnType("int")
                .IsRequired();

            return builder;
        }

        public static ModelBuilder BuildRelationshipTable(this ModelBuilder builder)
        {
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

            return builder;
        }

        public static ModelBuilder BuildPostTable(this ModelBuilder builder)
        {
            builder.Entity<Post>().ToTable(nameof(Post));
            builder.Entity<Post>().HasKey(p => p.PostId);
            builder.Entity<Post>().Property(p => p.PostId).ValueGeneratedOnAdd();
            builder.Entity<Post>().Property(p => p.Title)
                .HasColumnName(nameof(Post.Title))
                .HasColumnType("varchar(100)")
                .IsRequired();
            builder.Entity<Post>().Property(p => p.Content)
                .HasColumnName(nameof(Post.Content))
                .HasColumnType("text")
                .IsRequired();
            builder.Entity<Post>().Property(p => p.CreatedDate)
                .HasColumnName(nameof(Post.CreatedDate))
                .HasColumnType("timestamp")
                .IsRequired();
            builder.Entity<Post>().Property(p => p.UpdatedDate)
                .HasColumnName(nameof(Post.UpdatedDate))
                .HasColumnType("timestamp")
                .IsRequired(false);  
            builder.Entity<Post>().Property(p => p.AppUserId)
                .HasColumnName(nameof(Post.AppUserId))
                .HasColumnType("int")
                .IsRequired();

            return builder;
        }

        public static ModelBuilder BuildTableRelationships(this ModelBuilder builder)
        {
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
            builder.Entity<AppUser>()
                .HasMany(appUser => appUser.Posts)
                .WithOne(post => post.AppUser)
                .HasForeignKey(post => post.AppUserId);

            return builder;
        }
    }
}