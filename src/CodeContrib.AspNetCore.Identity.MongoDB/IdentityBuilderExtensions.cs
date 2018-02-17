﻿namespace CodeContrib.AspNetCore.Identity.MongoDB
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using global::MongoDB.Driver;
    using System;
    using System.Threading;

    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddMongoDBStores<TUser, TRole>(this IdentityBuilder builder, MongoUrl url, CancellationToken cancellationToken = default(CancellationToken))
            where TRole : IdentityRole
            where TUser : IdentityUser
        {
            MongoDBConfigurator.Configure();

            var client = new MongoClient(url);

            if (url.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", url.Url);
            }

            var database = client.GetDatabase(url.DatabaseName);

            if (typeof(TUser) != builder.UserType)
            {
                var message = $"The TUser type passed into AddIdentity ({builder.UserType}) doesn't match the type passed into AddMongoDBStores ({typeof(TUser)}).";

                throw new ArgumentException(message);
            }

            if (typeof(TRole) != builder.RoleType)
            {
                var message = $"The TUser type passed into AddIdentity ({builder.RoleType}) doesn't match the type passed into AddMongoDBStores ({typeof(TRole)}).";

                throw new ArgumentException(message);
            }

            var rolesCollection = database.GetCollection<TRole>(CollectionNames.Roles);
            var usersCollection = database.GetCollection<TUser>(CollectionNames.Users);

            builder.Services.AddSingleton<IRoleStore<TRole>>(sp => new RoleStore<TRole>(rolesCollection, sp.GetRequiredService<IdentityErrorDescriber>()));
            builder.Services.AddSingleton<IUserStore<TUser>>(sp => new UserStore<TUser>(usersCollection, sp.GetRequiredService<IdentityErrorDescriber>()));

            IndexConfigurator.CreateNormalizedEmailIndex(usersCollection, cancellationToken);
            IndexConfigurator.CreateNormalizedRoleNameIndex(rolesCollection, cancellationToken);
            IndexConfigurator.CreateNormalizedUserNameIndex(usersCollection, cancellationToken);

            return builder;
        }
    }
}
