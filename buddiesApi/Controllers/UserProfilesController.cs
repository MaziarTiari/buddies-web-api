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
            return base.Get(userId);
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
    }
}
