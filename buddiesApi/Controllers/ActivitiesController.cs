using System.Collections.Generic;
using buddiesApi.Hubs;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace buddiesApi.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : Controller<Activity, ActivityService> {

        private IHubContext<ActivityHub> hubContext;
        private UserProfileService userProfileService;

        public ActivitiesController(
                ActivityService service,
                UserProfileService userProfileService,
                IHubContext<ActivityHub> hubContext) : base(service) {
            this.hubContext = hubContext;
            this.userProfileService = userProfileService;
        }

        [HttpGet("my-activities")]
        public ActionResult<List<Activity>> GetUsersActivities() {
            return service.GetUsersActivities(ClientsUserId);
        }

        [HttpGet("offers")]
        public ActionResult<List<ForeignActivity>> GetForeignActivities() {
            return service.GetForeignActivities(ClientsUserId);
        }

        [HttpPut("{id:length(24)}")]
        public override ActionResult Replace(string id, Activity activity) {
            if (activity.UserId != ClientsUserId) {
                return Unauthorized();
            }
            hubContext.Clients.Group(id).SendAsync("updateActivity", activity);
            return base.Replace(id, activity);
        }

        [HttpPost]
        public override ActionResult Create(Activity activity) {
            if (activity.UserId != ClientsUserId) {
                return Unauthorized();
            } 
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

        [HttpPost("apply/{activityId:length(24)}")]
        public ActionResult Apply(string activityId) {
            Activity activity = service.Get(activityId);
            if (activity == null) {
                return new NotFoundResult();
            } else if (activity.ApplicantUserIds.Contains(ClientsUserId)) {
                return new StatusCodeResult(900);
            }
            activity.ApplicantUserIds.Add(ClientsUserId);
            _ = hubContext.Clients.Group(activity.Id).SendAsync(
                "newApplicant",
                new { AplicantId = ClientsUserId, ActivityId = activityId }
            );
            return base.Replace(activityId, activity);
        }

        [HttpPost("hide/{activityId:length(24)}")]
        public ActionResult Hide(string activityId) {
            if (activityId == null) {
                return new BadRequestResult();
            }
            service.HideActivity(activityId, ClientsUserId);
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
            if (activity.UserId != ClientsUserId) {
                return Unauthorized();
            }
            return userProfileService.GetUserAvatars(activity.ApplicantUserIds);
        }

        [HttpPost("accept/{activityId:length(24)}")]
        public ActionResult AcceptAppllications(string activityId, List<string> userIds) {
            Activity activity = service.Get(activityId);
            if (activity == null) {
                return new NotFoundResult();
            }
            if (activity.UserId == ClientsUserId) {
                return Unauthorized();
            }
            userIds.ForEach(userId => {
                activity.ApplicantUserIds.Remove(userId);
                activity.MemberUserIds.Add(userId);
            });
            hubContext.Clients
                .Group(activityId).SendAsync("updateActivity", activity);
            return base.Replace(activity.Id, activity);
        }

        [HttpPost("reject/{activityId:length(24)}")]
        public ActionResult RejectAppllications(string activityId, List<string> userIds) {
            Activity activity = service.Get(activityId);
            if (activity == null) {
                return new NotFoundResult();
            }
            if (activity.UserId == ClientsUserId) {
                return Unauthorized();
            }
            userIds.ForEach(userId => {
                activity.ApplicantUserIds.Remove(userId);
            });
            hubContext.Clients
                .Group(activityId).SendAsync("updateActivity", activity);
            return base.Replace(activity.Id, activity);
        }
    }
}
