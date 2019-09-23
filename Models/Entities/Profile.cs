using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace rgnl_server.Models.Entities
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Tags { get; set; }
        public string Url { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public int AppUserId { get; set; }
        [JsonIgnore]
        public virtual AppUser AppUser { get; set; }
    }
}