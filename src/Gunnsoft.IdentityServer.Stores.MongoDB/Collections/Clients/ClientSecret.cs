using System;

namespace Gunnsoft.IdentityServer.Stores.MongoDB.Collections.Clients
{
    public class ClientSecret
    {
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = "SharedSecret";
        public string Value { get; set; }
    }
}