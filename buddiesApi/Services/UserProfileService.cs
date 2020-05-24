using buddiesApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace buddiesApi.Services
{
    public class UserProfileService : Service<UserProfile>
    {

        public UserProfileService(IBuddiesDbContext settings) : base(settings)
        {
            this.collection =
                base.GetDatabase.GetCollection<UserProfile>(settings.UserProfilesCollectionName);
        }

        public override void Create(UserProfile obj) => collection.InsertOne(obj);

        public override List<UserProfile> Get() => collection.Find(u => true).ToList();

        public override UserProfile Get(string id) =>
            collection.Find(u => u.Id == id).FirstOrDefault();

        public override void Remove(UserProfile newObj) =>
            collection.DeleteOne(u => u.Id == newObj.Id);

        public override void Remove(string id) => collection.DeleteOne(u => u.Id == id);

        public override void Update(string id, UserProfile newObj) =>
            collection.ReplaceOne(u => u.Id == id, newObj);
    }
}
