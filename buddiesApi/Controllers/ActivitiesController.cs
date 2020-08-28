using System.Collections.Generic;
using buddiesApi.Hubs;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace buddiesApi.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : CrudController<Activity, ActivityService> {

        private IHubContext<ActivityHub> hubContext;
        private UserProfileService userProfileService;

        public ActivitiesController(
                ActivityService service,
                UserProfileService userProfileService,
                IHubContext<ActivityHub> hubContext) : base(service) {
            this.hubContext = hubContext;
            this.userProfileService = userProfileService;
        }

        [HttpGet("user/{userId:length(24)}")]
        public ActionResult<List<Activity>> GetUsersActivities(string userId) {
            return service.GetUsersActivities(userId);
        }

        [HttpGet("exclude/{userId:length(24)}")]
        public ActionResult<List<ForeignActivity>> GetForeignActivities(string userId) {
            return service.GetForeignActivities(userId);
        }

        [HttpPut("{id:length(24)}")]
        public override ActionResult Update(string id, Activity activity) {
            hubContext.Clients.Group(id).SendAsync("updateActivity", activity);
            return base.Update(id, activity);
        }

        [HttpPost]
        public override ActionResult<Activity> Create(Activity activity) {
            List<string> userIds = new List<string>();
            userIds.Add(activity.UserId);
            UserAvatar userAvatar = userProfileService.GetUserAvatars(userIds)[0];
            ForeignActivity othersActivity = new ForeignActivity();
            List<string> activityPropNames = new List<string>();
            foreach (PropertyInfo p in typeof(Activity).GetProperties()) {
                activityPropNames.Add(p.Name);
            }
            foreach (PropertyInfo p in othersActivity.GetType().GetProperties()) {
                if (activityPropNames.Contains(p.Name)) {
                    p.SetValue(othersActivity, p.GetValue(activity));
                }
            }
            othersActivity.Firstname = userAvatar.Firstname;
            othersActivity.Username = userAvatar.Username;
            othersActivity.Lastname = userAvatar.Lastname;
            othersActivity.Avatar = userAvatar.Avatar;

            hubContext.Clients.All.SendAsync("newActivity", othersActivity);
            return base.Create(activity);
        }

        [HttpPost("apply")]
        public ActionResult Apply(ActivityRequest request) {
            Activity activity = service.Get(request.ActivityId);
            if (activity == null) {
                return new NotFoundResult();
            } else if (activity.ApplicantUserIds.Contains(request.ApplicantId)) {
                return new StatusCodeResult(900);
            }
            activity.ApplicantUserIds.Add(request.ApplicantId);
            hubContext.Clients.Group(activity.UserId).SendAsync("newApplicant", request);
            return base.Update(request.ActivityId, activity);
        }

        [HttpPost("hide")]
        public ActionResult Hide(ActivityRequest request) {
            if (request.ActivityId == null) {
                return new BadRequestResult();
            }
            service.HideActivity(request);
            return new NoContentResult();
        }

        [HttpGet("members/{activityId:length(24)}")]
        public ActionResult<List<UserAvatar>> GetMembers(string activityId) {
            Activity activity = service.Get(activityId);
            return userProfileService.GetUserAvatars(activity.MemberUserIds);
        }

        [HttpGet("applicants/{activityId:length(24)}")]
        public ActionResult<List<UserAvatar>> GetApplicants(string activityId) {
            Activity activity = service.Get(activityId);
            return userProfileService.GetUserAvatars(activity.ApplicantUserIds);
        }

        [HttpPost("accept/{activityId:length(24)}")]
        public ActionResult AcceptAppllications(string activityId, List<string> userIds) {
            Activity activity = service.Get(activityId);
            if (activity == null) {
                return new NotFoundResult();
            }
            userIds.ForEach(userId => {
                activity.ApplicantUserIds.Remove(userId);
                activity.MemberUserIds.Add(userId);
            });
            hubContext.Clients
                .Group(activityId).SendAsync("updateActivity", activity);
            return base.Update(activity.Id, activity);
        }

        [HttpPost("reject/{activityId:length(24)}")]
        public ActionResult DenyAppllications(string activityId, List<string> userIds) {
            Activity activity = service.Get(activityId);
            if (activity == null) {
                return new NotFoundResult();
            }
            userIds.ForEach(userId => {
                activity.ApplicantUserIds.Remove(userId);
            });
            return base.Update(activity.Id, activity);
        }
    }
}
