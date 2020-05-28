using System;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services
{
    public class UserProfileService : Service<UserProfile>
    {
        public UserProfileService(IBuddiesDbContext settings) : base(settings)
        {
            this.collection = base.GetDatabase.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);
        }

        public UserProfile GetByUsername(string username) =>
            collection.Find<UserProfile>(user => user.Username == username)
            .FirstOrDefault();
    }
}
