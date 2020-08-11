using System;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services {
    public class ActivityService : Service<Activity> {

        public ActivityService(IBuddiesDbContext settings) : base(settings) {
            collection = GetDatabase.GetCollection<Activity>(
                settings.ActivitiesCollectionName);
        }

        public override Activity Get(string userId) {
            return collection.Find(o => o.UserId == userId).FirstOrDefault();
        }
    }
}
