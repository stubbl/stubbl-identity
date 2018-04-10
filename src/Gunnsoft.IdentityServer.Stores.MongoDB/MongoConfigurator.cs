using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.PersistedGrants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public static class MongoConfigurator
    {
        public static void Configure()
        {
            const string conventionName = "gunnsoft-identityserver-stores-mongodb";
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Remove(conventionName);
            ConventionRegistry.Register(conventionName, conventionPack, t => true);

            if (!BsonClassMap.IsClassMapRegistered(typeof(PersistedGrant)))
            {
                BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}