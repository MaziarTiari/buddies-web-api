using System;
using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Driver;
using System.Linq;
using System.Collections;
using MongoDB.Bson;

namespace buddiesApi.Services {
    public class ActivityService : Service<Activity> {
        private IMongoCollection<UserProfile> userProfileCollection;
        public ActivityService(IBuddiesDbContext settings) : base(settings) {
            collection = GetDatabase.GetCollection<Activity>(
                settings.ActivitiesCollectionName);
            userProfileCollection = GetDatabase.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);
        }

        public List<Activity> GetUsersActivities(string userId) {
            return collection.Find(a => a.UserId == userId).ToList<Activity>();
        }

        public List<OthersActivity> GetOthersActivities(string userId) {
            var restult = from a in collection.AsQueryable().Where(a => a.UserId != userId)
                          join u in userProfileCollection.AsQueryable()
                          on a.UserId equals u.UserId into user
                          select new OthersActivity {
                              Id = a.Id,
                              ApplicantUserIds = a.ApplicantUserIds,
                              Description = a.Description,
                              Title = a.Title,
                              ApplicationDeadline = a.ApplicationDeadline,
                              EndDate = a.EndDate,
                              Image = a.Image,
                              Location = a.Location,
                              MaxMember = a.MaxMember,
                              MemberUserIds = a.MemberUserIds,
                              StartDate = a.StartDate,
                              Tags = a.Tags,
                              Visibility = a.Visibility,
                              UserId = a.UserId,
                              Firstname = user.First().Firstname,
                              Lastname = user.First().Lastname,
                              Username = user.First().Username,
                              Avatar = user.First().Avatar
                          };

            return restult.ToList();
        }
    }
}
