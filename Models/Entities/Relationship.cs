namespace rgnl_server.Models.Entities
{
    public class Relationship
    {
        public int RelationshipId { get; set; }
        public int FollowerId { get; set; }
        public virtual AppUser Follower { get; set; }
        public int FolloweeId { get; set; }
        public virtual AppUser Followee { get; set; }
    }
}