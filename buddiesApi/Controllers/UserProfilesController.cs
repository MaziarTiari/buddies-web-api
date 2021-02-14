using System.Collections.Generic;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController: Controller<UserProfile, UserProfileService>
    {
        public UserProfilesController(UserProfileService userProfileService)
            : base(userProfileService) { }

        [HttpGet]
        public ActionResult<UserProfile> GetUsersProfile() {
            return Get(ClientsUserId);
        }

        [HttpGet("{userId:length(24)}")]
        public override ActionResult<UserProfile> Get(string userId) {
            UserProfile userProfile = service.Get(userId);
            if (userProfile == null) {
                return new NotFoundResult();
            }
            return userProfile;
        }

        [HttpGet("username/{username}")]
        public ActionResult<UserProfile> GetByUsername(string username)
        {
            UserProfile userProfile = service.GetByUsername(username);
            if (userProfile == null) {
                return new NotFoundResult();
            }
            return userProfile;
        }

        [HttpPost]
        public override ActionResult Create(UserProfile userProfile)
        {
            if (ClientsUserId != userProfile.UserId) {
                return Unauthorized();
            }
            var user = service.GetByUsername(userProfile.Username);
            if(user != null) return new ConflictResult();
            service.Create(userProfile);
            return new CreatedResult("UserProfiles", userProfile);
        }

        [HttpGet("user-avatar/{userId:length(24)}")]
        public ActionResult<UserAvatar> GetUserAvatar(string userId) {
            List<string> userIds = new List<string>();
            userIds.Add(userId);
            return service.GetUserAvatars(userIds)[0];
        }

        [HttpPost("user-avatars")]
        public ActionResult<List<UserAvatar>> GetUserAvatars(List<string> userIds) {
            return service.GetUserAvatars(userIds);
        }
    }
}
