﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Driver;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public class MongoPersistedGrantStore : IPersistedGrantStore
    {
        private readonly IMongoCollection<PersistedGrant> _persistedGrantsCollection;

        public MongoPersistedGrantStore(IMongoCollection<PersistedGrant> persistedGrantsCollection)
        {
            _persistedGrantsCollection = persistedGrantsCollection;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            return await _persistedGrantsCollection.Find(pg => pg.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await _persistedGrantsCollection.Find(pg => pg.Key == key)
                .SortByDescending(pg => pg.CreationTime)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            await _persistedGrantsCollection.DeleteManyAsync(pg =>
                pg.SubjectId == subjectId && pg.ClientId == clientId);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            await _persistedGrantsCollection.DeleteManyAsync(pg =>
                pg.SubjectId == subjectId && pg.ClientId == clientId && pg.Type == type);
        }

        public async Task RemoveAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await _persistedGrantsCollection.DeleteManyAsync(pg => pg.Key == key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            if (grant == null)
            {
                throw new ArgumentNullException(nameof(grant));
            }

            await _persistedGrantsCollection.InsertOneAsync(grant);
        }
    }
}