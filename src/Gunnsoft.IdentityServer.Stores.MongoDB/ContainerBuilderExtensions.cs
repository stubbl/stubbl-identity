using System;
using Autofac;
using IdentityServer4.Models;
using MongoDB.Driver;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public static partial class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddMongoPersistedGrantStore(this ContainerBuilder extended, MongoUrl mongoUrl)
        {
            MongoConfigurator.Configure();

            var client = new MongoClient(mongoUrl);

            if (mongoUrl.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", mongoUrl.Url);
            }

            var database = client.GetDatabase(mongoUrl.DatabaseName);

            var persistedGrantsCollection = database.GetCollection<PersistedGrant>(CollectionNames.PersistedGrants);

            extended.Register(cc => new MongoPersistedGrantStore(persistedGrantsCollection))
                .InstancePerDependency();

            return extended;
        }
    }
}
