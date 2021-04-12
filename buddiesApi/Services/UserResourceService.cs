using System;
using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services {
    public class UserResourceService<T> : Service<T> where T : IMongoDbUserDocument {
        public UserResourceService(IBuddiesDbContext settings) : base(settings) {}

        public override T Get(string userId) {
            return collection.Find(r => r.UserId == userId).First();
        }

        public virtual List<T> GetMany(string userId) {
            return collection.Find(i => i.UserId == userId).ToList();
        }
    }
}
