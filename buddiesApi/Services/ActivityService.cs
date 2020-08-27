using System;
using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Driver;
using System.Linq;

namespace buddiesApi.Services {
    public class ActivityService : Service<Activity> {
        private IMongoCollection<UserProfile> userProfileCollection;
        private IMongoCollection<ActivityMeta> activityMetaCollection;

        public ActivityService(IBuddiesDbContext settings) : base(settings) {
            collection = GetDatabase.GetCollection<Activity>(
                settings.ActivitiesCollectionName);

            userProfileCollection = GetDatabase.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);

            activityMetaCollection = GetDatabase.GetCollection<ActivityMeta>(
                settings.ActivityMetaCollectionName);
        }

        public List<Activity> GetUsersActivities(string userId) {
            return collection.Find(a => a.UserId == userId).ToList<Activity>();
        }

        public void HideActivity(ActivityRequest request) {
            ActivityMeta meta = new ActivityMeta();
            meta = activityMetaCollection
                .Find(a => a.UserId == request.ApplicantId)
                .FirstOrDefault();
            if (meta == null) {
                meta.UserId = request.ApplicantId;
                meta.HiddenActivityIds = new List<string>();
                meta.HiddenActivityIds.Add(request.ActivityId);
                activityMetaCollection.InsertOne(meta);
            } else {
                meta.HiddenActivityIds.Add(request.ActivityId);
                activityMetaCollection.ReplaceOne(a => a.Id == meta.Id, meta);
            }
        }

        public List<ForeignActivity> GetForeignActivities(string userId) {
            ActivityMeta meta = activityMetaCollection
                .Find(a => a.UserId == userId)
                .FirstOrDefault();
            if (meta == null) {
                meta = new ActivityMeta();
                meta.HiddenActivityIds = new List<string>();
            }

            var restult = from a in collection.AsQueryable()
                          .Where(a =>
                                a.UserId != userId
                                && !( a.ApplicantUserIds.Contains(userId)
                                        || a.MemberUserIds.Contains(userId)
                                        || meta.HiddenActivityIds.Contains(a.Id))
                          )
                          join u in userProfileCollection.AsQueryable()
                          on a.UserId equals u.UserId into user
                          select new ForeignActivity {
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
