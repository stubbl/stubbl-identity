using System;
using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.Clients;
using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.PersistedGrants;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public static partial class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoClientStore(this IIdentityServerBuilder extended, MongoUrl mongoUrl)
        {
            MongoConfigurator.Configure();

            var client = new MongoClient(mongoUrl);

            if (mongoUrl.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", mongoUrl.Url);
            }

            var database = client.GetDatabase(mongoUrl.DatabaseName);

            var clientsCollection = database.GetCollection<Client>(CollectionNames.Clients);

            extended.Services.AddTransient(cc => new MongoClientStore(clientsCollection));

            return extended;
        }

        public static IIdentityServerBuilder AddMongoPersistedGrantStore(this IIdentityServerBuilder extended, MongoUrl mongoUrl)
        {
            MongoConfigurator.Configure();

            var client = new MongoClient(mongoUrl);

            if (mongoUrl.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", mongoUrl.Url);
            }

            var database = client.GetDatabase(mongoUrl.DatabaseName);

            var persistedGrantsCollection = database.GetCollection<PersistedGrant>(CollectionNames.PersistedGrants);

            extended.Services.AddTransient(cc => new MongoPersistedGrantStore(persistedGrantsCollection));

            return extended;
        }
    }
}
