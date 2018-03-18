using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public class IdentityUser
    {
        public IdentityUser()
        {
            Id = ObjectId.GenerateNewId();

            Claims = new List<IdentityUserClaim>();
            Logins = new List<IdentityUserLogin>();
            Roles = new List<string>();
            Tokens = new List<IdentityUserToken>();
        }

        public virtual int AccessFailedCount { get; set; }
        public List<IdentityUserClaim> Claims { get; internal set; }
        public ObjectId Id { get; internal set; }
        public string EmailAddress { get; set; }
        public bool EmailAddressConfirmed { get; set; }
        public virtual DateTime? LockoutEnd { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public List<IdentityUserLogin> Logins { get; internal set; }
        public string NormalizedEmailAddress { get; set; }
        public string NormalizedUsername { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; internal set; }
        public string SecurityStamp { get; set; }
        public List<IdentityUserToken> Tokens { get; internal set; }
        public bool TwoFactorEnabled { get; set; }
        public string Username { get; set; }
    }
}