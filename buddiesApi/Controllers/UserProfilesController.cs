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
    public class UserProfileService : Service<UserProfile>
    {
        public UserProfileService(IBuddiesDbContext settings) : base(settings)
        {
            this.collection = base.GetDatabase.GetCollection<UserProfile>(
                settings.UserProfilesCollectionName);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController: CrudController<UserProfile, UserProfileService>
    {
        public UserProfilesController(UserProfileService userProfileService)
            : base(userProfileService) { }

    }
}
