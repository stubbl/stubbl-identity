using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    /// <summary>
    ///     Provides an abstraction for a store which manages user accounts.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    public class UserStore<TUser> : IQueryableUserStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserClaimStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserLoginStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserRoleStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserTwoFactorStore<TUser>
        where TUser : IdentityUser
    {
        private readonly IdentityErrorDescriber _errorDescriber;
        private readonly IMongoCollection<TUser> _usersCollection;

        public UserStore(IMongoCollection<TUser> usersCollection,
            IdentityErrorDescriber errorDescriber)
        {
            _usersCollection = usersCollection;
            _errorDescriber = errorDescriber;
        }

        public IQueryable<TUser> Users => _usersCollection.AsQueryable();

        /// <summary>
        ///     Creates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the
        ///     <see cref="IdentityResult" /> of the creation operation.
        /// </returns>
        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _usersCollection.InsertOneAsync(user, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        ///     Deletes the specified <paramref name="user" /> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the
        ///     <see cref="IdentityResult" /> of the update operation.
        /// </returns>
        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await _usersCollection.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);

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
        ///     Finds and returns a user, if any, who has the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the user matching the specified
        ///     <paramref name="userId" /> if it exists.
        /// </returns>
        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (!ObjectId.TryParse(userId, out var objectId))
            {
                return null;
            }

            return await _usersCollection.Find(u => u.Id == objectId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the user matching the specified
        ///     <paramref name="normalizedUserName" /> if it exists.
        /// </returns>
        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (normalizedUserName == null)
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            return await _usersCollection.Find(u => u.NormalizedUsername == normalizedUserName)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Gets the normalized user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the normalized user name for
        ///     the specified <paramref name="user" />.
        /// </returns>
        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUsername);
        }

        /// <summary>
        ///     Gets the user identifier for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the identifier for the
        ///     specified <paramref name="user" />.
        /// </returns>
        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id.ToString());
        }

        /// <summary>
        ///     Gets the user name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the name for the specified
        ///     <paramref name="user" />.
        /// </returns>
        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Username);
        }

        /// <summary>
        ///     Sets the given normalized name for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="normalizedName">The normalized name to set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetNormalizedUserNameAsync(TUser user, string normalizedUserName,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedUsername = normalizedUserName;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets the given <paramref name="userName" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="userName">The user name to set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Username = userName;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Updates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the
        ///     <see cref="IdentityResult" /> of the update operation.
        /// </returns>
        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result =
                await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user,
                    cancellationToken: cancellationToken);

            if (result.ModifiedCount != 1)
            {
                return IdentityResult.Failed(_errorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        ///     Returns the token value.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var tokenValue = user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name)
                ?.Value;

            return Task.FromResult(tokenValue);
        }

        /// <summary>
        ///     Deletes a token for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets the token value for a particular user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The value of the token.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }


            var existingToken = user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);

            if (existingToken != null)
            {
                existingToken.Value = value;

                return Task.CompletedTask;
            }

            user.Tokens.Add(new IdentityUserToken
            {
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            });

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Add claims to a user as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The collection of <see cref="Claim" />s to add.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var userClaims = claims.Where(c => !user.Claims.Any(uc => uc.Type == c.Type && uc.Value == c.Value))
                .ToUserClaims();

            user.Claims.AddRange(userClaims);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Gets a list of <see cref="Claim" />s to be belonging to the specified <paramref name="user" /> as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="user">The role whose claims to retrieve.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a list of <see cref="Claim" />
        ///     s.
        /// </returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<Claim> claims = user.Claims.Select(uc => new Claim(uc.Type, uc.Value))
                .ToList();

            return Task.FromResult(claims);
        }

        /// <summary>
        ///     Returns a list of users who contain the specified <see cref="Claim" />.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a list of
        ///     <typeparamref name="TUser" /> who
        ///     contain the specified claim.
        /// </returns>
        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return await _usersCollection.Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        ///     Removes the specified <paramref name="claims" /> from the given <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to remove the specified <paramref name="claims" /> from.</param>
        /// <param name="claims">A collection of <see cref="Claim" />s to remove.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                user.Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Replaces the given <paramref name="claim" /> on the specified <paramref name="user" /> with the
        ///     <paramref name="newClaim" />
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim to replace.</param>
        /// <param name="newClaim">The new claim to replace the existing <paramref name="claim" /> with.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            if (!user.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            {
                return;
            }

            await RemoveClaimsAsync(user, new[] {claim}, cancellationToken);
            await AddClaimsAsync(user, new[] {claim}, cancellationToken);
        }

        /// <summary>
        ///     Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The task object containing the results of the asynchronous lookup operation, the user if any associated with the
        ///     specified normalized email address.
        /// </returns>
        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (normalizedEmail == null)
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            return await _usersCollection.Find(u => u.NormalizedEmailAddress == normalizedEmail)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Gets the email address for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The task object containing the results of the asynchronous operation, the email address for the specified
        ///     <paramref name="user" />.
        /// </returns>
        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailAddress);
        }

        /// <summary>
        ///     Gets a flag indicating whether the email address for the specified <paramref name="user" /> has been verified, true
        ///     if the email address is verified otherwise
        ///     false.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The task object containing the results of the asynchronous operation, a flag indicating whether the email address
        ///     for the specified <paramref name="user" />
        ///     has been confirmed or not.
        /// </returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailAddressConfirmed);
        }

        /// <summary>
        ///     Returns the normalized email for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The task object containing the results of the asynchronous lookup operation, the normalized email address if any
        ///     associated with the specified user.
        /// </returns>
        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedEmailAddress);
        }

        /// <summary>
        ///     Sets the <paramref name="email" /> address for a <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="email">The email to set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailAddress = email;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets the flag indicating whether the specified <paramref name="user" />'s email address has been confirmed or not.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be set.</param>
        /// <param name="confirmed">
        ///     A flag indicating if the email address has been confirmed, true if the address is confirmed
        ///     otherwise false.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailAddressConfirmed = confirmed;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets the normalized email for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedEmailAddress = normalizedEmail;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Retrieves the current failed access count for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose failed access count should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the failed access count.</returns>
        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        ///     Retrieves a flag indicating whether user lockout can enabled for the specified user.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be returned.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, true if a user can be locked out, otherwise
        ///     false.
        /// </returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        ///     Gets the last <see cref="DateTimeOffset" /> a user's last lockout expired, if any.
        ///     Any time in the past should be indicates a user is not locked out.
        /// </summary>
        /// <param name="user">The user whose lockout date should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        ///     <see cref="DateTimeOffset" /> containing the last time
        ///     a user's lockout expired, if any.
        /// </returns>
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            DateTimeOffset? lockoutEndDateUtc = user.LockoutEnd;

            return Task.FromResult(lockoutEndDateUtc);
        }

        /// <summary>
        ///     Records that a failed access has occurred, incrementing the failed access count.
        /// </summary>
        /// <param name="user">The user whose cancellation count should be incremented.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the incremented failed access
        ///     count.
        /// </returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        ///     Resets a user's failed access count.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        /// <remarks>This is typically called after the account is successfully accessed.</remarks>
        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Set the flag indicating if the specified <paramref name="user" /> can be locked out.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be set.</param>
        /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a
        ///     user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">
        ///     The <see cref="DateTimeOffset" /> after which the <paramref name="user" />'s lockout should
        ///     end.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd?.UtcDateTime;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Adds an external <see cref="UserLoginInfo" /> to the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The external <see cref="UserLoginInfo" /> to add to the specified <paramref name="user" />.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            if (user.Logins.Any(l =>
                l.LoginProvider == login.LoginProvider && l.ProviderDisplayName == login.ProviderDisplayName &&
                l.ProviderKey == login.ProviderKey))
            {
                return Task.CompletedTask;
            }

            user.Logins.Add(new IdentityUserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey
            });

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Retrieves the user associated with the specified login provider and login provider key.
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey" />.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider" /> to identify a user.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> for the asynchronous operation, containing the user, if any which matched the specified
        ///     login provider and key.
        /// </returns>
        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            return await _usersCollection.Find(u =>
                    u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves the associated logins for the specified
        ///     <param ref="user" />
        ///     .
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> for the asynchronous operation, containing a list of <see cref="UserLoginInfo" /> for the
        ///     specified <paramref name="user" />, if any.
        /// </returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<UserLoginInfo> logins = user.Logins
                .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName))
                .ToList();

            return Task.FromResult(logins);
        }

        /// <summary>
        ///     Attempts to remove the provided login information from the specified <paramref name="user" />.
        ///     and returns a flag indicating whether the removal succeed or not.
        /// </summary>
        /// <param name="user">The user to remove the login information from.</param>
        /// <param name="loginProvider">The login provide whose information should be removed.</param>
        /// <param name="providerKey">The key given by the external login provider for the specified user.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            user.Logins.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Gets the password hash for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password hash to retrieve.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, returning the password hash for the
        ///     specified <paramref name="user" />.
        /// </returns>
        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        ///     Gets a flag indicating whether the specified <paramref name="user" /> has a password.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, returning true if the specified
        ///     <paramref name="user" /> has a password
        ///     otherwise false.
        /// </returns>
        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        /// <summary>
        ///     Sets the password hash for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password hash to set.</param>
        /// <param name="passwordHash">The password hash to set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Gets the telephone number, if any, for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the user's telephone number, if
        ///     any.
        /// </returns>
        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        ///     Gets a flag indicating whether the specified <paramref name="user" />'s telephone number has been confirmed.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, returning true if the specified
        ///     <paramref name="user" /> has a confirmed
        ///     telephone number otherwise false.
        /// </returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        ///     Sets the telephone number for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets a flag indicating if the specified <paramref name="user" />'s phone number has been confirmed.
        /// </summary>
        /// <param name="user">The user whose telephone number confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Add a the specified <paramref name="user" /> to the named role.
        /// </summary>
        /// <param name="user">The user to add to the named role.</param>
        /// <param name="roleName">The name of the role to add the user to.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            if (user.Roles.Contains(roleName))
            {
                return Task.CompletedTask;
            }

            user.Roles.Add(roleName);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Gets a list of role names the specified <paramref name="user" /> belongs to.
        /// </summary>
        /// <param name="user">The user whose role names to retrieve.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing a list of role names.</returns>
        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<string> roles = user.Roles;

            return Task.FromResult(roles);
        }

        /// <summary>
        ///     Returns a list of Users who are members of the named role.
        /// </summary>
        /// <param name="roleName">The name of the role whose membership should be returned.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing a list of users who are in the named
        ///     role.
        /// </returns>
        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return await _usersCollection.Find(u => u.Roles.Contains(roleName))
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        ///     Returns a flag indicating whether the specified <paramref name="user" /> is a member of the give named role.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="roleName">The name of the role to be checked.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing a flag indicating whether the
        ///     specified <paramref name="user" /> is
        ///     a member of the named role.
        /// </returns>
        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return Task.FromResult(user.Roles.Contains(roleName));
        }

        /// <summary>
        ///     Add a the specified <paramref name="user" /> from the named role.
        /// </summary>
        /// <param name="user">The user to remove the named role from.</param>
        /// <param name="roleName">The name of the role to remove.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            user.Roles.RemoveAll(r => r == roleName);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Get the security stamp for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing the security stamp for the
        ///     specified <paramref name="user" />.
        /// </returns>
        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///     Sets the provided security <paramref name="stamp" /> for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="stamp">The security stamp to set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Returns a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or
        ///     not,
        ///     as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" /> that represents the asynchronous operation, containing a flag indicating whether the
        ///     specified
        ///     <paramref name="user" /> has two factor authentication enabled or not.
        /// </returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        ///     Sets a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        ///     as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">
        ///     A flag indicating whether the specified <paramref name="user" /> has two factor authentication
        ///     enabled.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
        ///     should be canceled.
        /// </param>
        /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }
    }
}