using System;
using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace buddiesApi.Services
{
    public class UserProfileService : Service<UserProfile>
    {
        public UserProfileService(IBuddiesDbContext settings) : base(settings)
        {
            this.collection = base.Database.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);
        }

        public override UserProfile Get(string userId) {
            return collection.Find(o => o.UserId == userId).FirstOrDefault();
        }

        public UserProfile GetByUsername(string username)
        {
            return collection.Find<UserProfile>(
                user => user.Username == username.ToLower()).FirstOrDefault();
        }

        public override void Create(UserProfile userProfile)
        {
            userProfile.Username = userProfile.Username.ToLower();
            base.Create(userProfile);
        }

        public override ReplaceOneResult Update(string id, UserProfile userProfile)
        {
            userProfile.Username = userProfile.Username.ToLower();
            return base.Update(id, userProfile);
        }

        public List<UserAvatar> GetUserAvatars(List<string> userIds) {
            var result = collection.AsQueryable()
                .Where(u => userIds.Contains(u.UserId))
                .Select(e => new UserAvatar {
                    UserId = e.UserId,
                    Username = e.Username,
                    Avatar = e.Avatar,
                    Firstname = e.Firstname,
                    Lastname = e.Lastname
                });
            return result.ToList();
        }
    }
}
