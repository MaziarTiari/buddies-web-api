﻿using System;
using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Driver;
using System.Linq;

namespace buddiesApi.Services {
    public class ActivityService : UserResourceService<Activity> {
        private IMongoCollection<UserProfile> userProfileCollection;
        private IMongoCollection<ActivityMeta> activityMetaCollection;

        public ActivityService(IBuddiesDbContext settings) : base(settings) {
            collection = Database.GetCollection<Activity>(
                settings.ActivitiesCollectionName);

            userProfileCollection = Database.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);

            activityMetaCollection = Database.GetCollection<ActivityMeta>(
                settings.ActivityMetaCollectionName);
        }

        public void HideActivity(string activityId, string userId) {
            ActivityMeta meta = activityMetaCollection
                .Find(a => a.UserId == userId)
                .FirstOrDefault();
            if (meta == null) {
                meta = new ActivityMeta {
                    UserId = userId,
                    HiddenActivityIds = new List<string>()
                };
                meta.HiddenActivityIds.Add(activityId);
                activityMetaCollection.InsertOne(meta);
            } else {
                meta.HiddenActivityIds.Add(activityId);
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
                              Image = a.Image,
                              Location = a.Location,
                              MaxMember = a.MaxMember,
                              MemberUserIds = a.MemberUserIds,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              StartTime = a.StartTime,
                              EndTime = a.EndTime,
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
