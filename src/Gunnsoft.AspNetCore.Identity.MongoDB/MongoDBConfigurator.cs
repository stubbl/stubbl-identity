namespace CodeContrib.AspNetCore.Identity.MongoDB
{
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Bson.Serialization.Conventions;

    public static class MongoDBConfigurator
    {
        public static void Configure()
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("stubbl-identity", conventionPack, t => true);

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
