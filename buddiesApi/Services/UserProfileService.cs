using System;
using buddiesApi.Models;

namespace buddiesApi.Services
{
    public class UserProfileService : Service<UserProfile>
    {
        public UserProfileService(IBuddiesDbContext settings) : base(settings)
        {
            this.collection = base.GetDatabase.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);
        }
    }
}
