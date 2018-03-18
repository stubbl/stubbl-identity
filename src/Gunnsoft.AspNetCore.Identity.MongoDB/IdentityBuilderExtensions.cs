using System;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddMongoIdentity<TUser, TRole>(this IdentityBuilder extended, MongoUrl mongoUrl,
            CancellationToken cancellationToken = default(CancellationToken))
            where TRole : IdentityRole
            where TUser : IdentityUser
        {
            MongoConfigurator.Configure();

            var client = new MongoClient(mongoUrl);

            if (mongoUrl.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", mongoUrl.Url);
            }

            var database = client.GetDatabase(mongoUrl.DatabaseName);

            if (typeof(TUser) != extended.UserType)
            {
                var message =
                    $"The TUser type passed into AddIdentity ({extended.UserType}) doesn't match the type passed into AddMongoDBStores ({typeof(TUser)}).";

                throw new ArgumentException(message);
            }

            if (typeof(TRole) != extended.RoleType)
            {
                var message =
                    $"The TUser type passed into AddIdentity ({extended.RoleType}) doesn't match the type passed into AddMongoDBStores ({typeof(TRole)}).";

                throw new ArgumentException(message);
            }

            var rolesCollection = database.GetCollection<TRole>(CollectionNames.Roles);
            var usersCollection = database.GetCollection<TUser>(CollectionNames.Users);

            extended.Services.AddSingleton<IRoleStore<TRole>>(sp =>
                new RoleStore<TRole>(rolesCollection, sp.GetRequiredService<IdentityErrorDescriber>()));
            extended.Services.AddSingleton<IUserStore<TUser>>(sp =>
                new UserStore<TUser>(usersCollection, sp.GetRequiredService<IdentityErrorDescriber>()));

            IndexConfigurator.CreateNormalizedEmailIndex(usersCollection, cancellationToken);
            IndexConfigurator.CreateNormalizedRoleNameIndex(rolesCollection, cancellationToken);
            IndexConfigurator.CreateNormalizedUserNameIndex(usersCollection, cancellationToken);

            return extended;
        }
    }
}