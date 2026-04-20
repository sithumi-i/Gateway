using AspNetCore.Identity.Mongo.Model;

namespace Gateway.BlindMatch.Models
{
    public class ApplicationUser : MongoUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
