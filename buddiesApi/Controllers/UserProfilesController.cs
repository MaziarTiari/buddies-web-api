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

        [HttpGet("username/{username:string}")]
        public ActionResult<UserProfile> GetByUsername(string username)
        {
            //var user = userProfileService.GetByUsername(username);
            //if (user == null) return new NotFoundResult();
            return userProfileService.GetByUsername(username);
        }
    }
}
