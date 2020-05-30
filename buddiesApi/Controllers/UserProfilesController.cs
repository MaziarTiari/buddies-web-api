using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using static buddiesApi.Helpers.SecurityManager;

namespace buddiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController: CrudController<UserProfile, UserProfileService>
    {
        UserProfileService userProfileService => (service as UserProfileService);

        public UserProfilesController(UserProfileService userProfileService)
            : base(userProfileService) { }

        [HttpGet("username/{username}")]
        public ActionResult<UserProfile> GetByUsername(string username)
        {
            return userProfileService.GetByUsername(username.ToLower());
        }

        [HttpPost]
        public override ActionResult<UserProfile> Create(UserProfile userProfile)
        {
            userProfile.Username = userProfile.Username.ToLower();
            var user = userProfileService.GetByUsername(userProfile.Username);
            if(user != null) return new ConflictResult();
            userProfileService.Create(userProfile);
            return new CreatedResult("UserProfiles", userProfile);
        }
    }
}
