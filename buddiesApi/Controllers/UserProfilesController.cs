using System.Collections.Generic;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController: CrudController<UserProfile, UserProfileService>
    {
        public UserProfilesController(UserProfileService userProfileService)
            : base(userProfileService) { }

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
            return service.GetByUsername(username);
        }

        [HttpPost]
        public override ActionResult<UserProfile> Create(UserProfile userProfile)
        {
            var user = service.GetByUsername(userProfile.Username);
            if(user != null) return new ConflictResult();
            service.Create(userProfile);
            return new CreatedResult("UserProfiles", userProfile);
        }

        [HttpGet("userAvatar/{userId:length(24)}")]
        public ActionResult<UserAvatar> GetUserAvatar(string userId) {
            List<string> userIds = new List<string>();
            userIds.Add(userId);
            return service.GetUserAvatars(userIds)[0];
        }

        [HttpPost("getUserAvatars")]
        public ActionResult<List<UserAvatar>> GetUserAvatars(List<string> userIds) {
            return service.GetUserAvatars(userIds);
        }
    }
}
