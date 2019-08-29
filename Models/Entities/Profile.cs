using System.ComponentModel.DataAnnotations;

namespace rgnl_server.Models.Entities
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Tags { get; set; }
        public string Url { get; set; }
        public string Address { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}