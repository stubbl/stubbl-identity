using System.Threading;
using MongoDB.Driver;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public static class IndexConfigurator
    {
        public static async void CreateNormalizedEmailIndex<TUser>(IMongoCollection<TUser> usersCollection,
            CancellationToken cancellationToken = default(CancellationToken))
            where TUser : IdentityUser
        {
            var index = Builders<TUser>.IndexKeys.Ascending(t => t.NormalizedEmailAddress);
            var options = new CreateIndexOptions
            {
                Unique = true
            };

            await usersCollection.Indexes.CreateOneAsync(index, options, cancellationToken);
        }

        public static async void CreateNormalizedRoleNameIndex<TRole>(IMongoCollection<TRole> rolesCollection,
            CancellationToken cancellationToken = default(CancellationToken))
            where TRole : IdentityRole
        {
            var index = Builders<TRole>.IndexKeys.Ascending(t => t.NormalizedName);
            var options = new CreateIndexOptions
            {
                Unique = true
            };

            await rolesCollection.Indexes.CreateOneAsync(index, options, cancellationToken);
        }

        public static async void CreateNormalizedUserNameIndex<TUser>(IMongoCollection<TUser> usersCollection,
            CancellationToken cancellationToken = default(CancellationToken))
            where TUser : IdentityUser
        {
            var index = Builders<TUser>.IndexKeys.Ascending(t => t.NormalizedUsername);
            var options = new CreateIndexOptions
            {
                Unique = true
            };

            await usersCollection.Indexes.CreateOneAsync(index, options, cancellationToken);
        }
    }
}