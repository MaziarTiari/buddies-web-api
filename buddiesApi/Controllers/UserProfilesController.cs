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
        public UserProfilesController(UserProfileService userProfileService)
            : base(userProfileService) { }

    }
}
