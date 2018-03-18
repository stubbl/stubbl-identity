using MongoDB.Bson;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public class IdentityRole
    {
        public IdentityRole()
        {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}