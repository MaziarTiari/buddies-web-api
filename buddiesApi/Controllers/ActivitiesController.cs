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
        public ActionResult<List<OthersActivity>> GetOthersActivities(string userId) {
            return service.GetOthersActivities(userId);
        }

        [HttpPut("{id:length(24)}")]
        public override ActionResult Update(string id, Activity newObj) {
            hubContext.Clients.Group(id).SendAsync("updateActivity", newObj);
            return base.Update(id, newObj);
        }

        [HttpPost]
        public override ActionResult<Activity> Create(Activity activity) {
            List<string> userIds = new List<string>();
            userIds.Add(activity.UserId);
            UserAvatar userAvatar = userProfileService.GetUserAvatars(userIds)[0];
            OthersActivity othersActivity = new OthersActivity();
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
        public ActionResult Apply(ActivityApply apply) {
            Activity activity = service.Get(apply.ActivityId);
            if (activity == null) {
                return new NotFoundResult();
            }
            activity.ApplicantUserIds.Add(apply.ApplicantId);
            service.Update(activity.Id, activity);
            hubContext.Clients
                .Group(activity.UserId).SendAsync("newApplicant", apply.ApplicantId);
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
    }
}
