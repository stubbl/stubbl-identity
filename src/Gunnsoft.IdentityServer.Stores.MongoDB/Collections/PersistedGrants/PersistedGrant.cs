using System;
using MongoDB.Bson;

namespace Gunnsoft.IdentityServer.Stores.MongoDB.Collections.PersistedGrants
{
    public class PersistedGrant
    {
        public string ClientId { get; set; }
        public string Data { get; set; }
        public DateTime? Expiration { get; set; }
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string SubjectId { get; set; }
    }
}
