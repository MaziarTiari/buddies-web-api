using System;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services {
    public abstract class UserResourceService<T> : Service<T> where T : IUsersDocument {
        public UserResourceService(IBuddiesDbContext settings) : base(settings) {}

        public virtual T GetForUserId(string userId) {
            return collection.Find(r => r.UserId == userId).First();
        }
    }
}
