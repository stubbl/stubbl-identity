using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private readonly IdentityErrorDescriber _errorDescriber;
        private readonly IMongoCollection<TRole> _rolesCollection;

        public RoleStore(IMongoCollection<TRole> rolesCollection,
            IdentityErrorDescriber errorDescriber = null)
        {
            _rolesCollection = rolesCollection;
            _errorDescriber = errorDescriber;
        }

        public IQueryable<TRole> Roles => _rolesCollection.AsQueryable();

        /// <summary>
        ///     Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _rolesCollection.InsertOneAsync(role, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        ///     Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var result = await _rolesCollection.DeleteOneAsync(r => r.Id == role.Id, cancellationToken);

            if (result.DeletedCount != 1)
            {
                return IdentityResult.Failed(_errorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        /// <summary>
        ///     Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that result of the look up.</returns>
        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (roleId == null)
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            if (!ObjectId.TryParse(roleId, out var objectId))
            {
                return null;
            }

            return await _rolesCollection.Find(r => r.Id == objectId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that result of the look up.</returns>
        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (normalizedRoleName == null)
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            return await _rolesCollection.Find(u => u.NormalizedName == normalizedRoleName)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Get a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that contains the name of the role.</returns>
        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        /// <summary>
        ///     Gets the ID for a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be returned.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that contains the ID of the role.</returns>
        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id.ToString());
        }

        /// <summary>
        ///     Gets the name of a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be returned.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that contains the name of the role.</returns>
        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        /// <summary>
        ///     Set a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be set.</param>
        /// <param name="normalizedName">The normalized name to set</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets the name of a role in the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = roleName;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>A <see cref="Task{TResult}" /> that represents the <see cref="IdentityResult" /> of the asynchronous query.</returns>
        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var result =
                await _rolesCollection.ReplaceOneAsync(r => r.Id == role.Id, role,
                    cancellationToken: cancellationToken);

            if (result.ModifiedCount != 1)
            {
                return IdentityResult.Failed(_errorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }
    }
}