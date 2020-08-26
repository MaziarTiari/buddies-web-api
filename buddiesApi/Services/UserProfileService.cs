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
            this.collection = base.GetDatabase.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);
        }

        public UserProfile GetByUsername(string username)
        {
            return collection.Find<UserProfile>(
                user => user.Username == username.ToLower()).FirstOrDefault();
        }

        public override UserProfile Get(string userId)
        {
            return collection.Find(o => o.UserId == userId).FirstOrDefault();
        }

        public override void Create(UserProfile obj)
        {
            obj.Username = obj.Username.ToLower();
            base.Create(obj);
        }

        public override ReplaceOneResult Update(string id, UserProfile newObj)
        {
            newObj.Username = newObj.Username.ToLower();
            return base.Update(id, newObj);
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
