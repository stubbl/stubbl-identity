using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public static class MongoConfigurator
    {
        public static void Configure()
        {
            const string conventionName = "gunnsoft-aspnetcore-identity-mongodb";
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Remove(conventionName);
            ConventionRegistry.Register(conventionName, conventionPack, t => true);

            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityRole)))
            {
                BsonClassMap.RegisterClassMap<IdentityRole>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityUser)))
            {
                BsonClassMap.RegisterClassMap<IdentityUser>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}