using System;
using System.Collections.Generic;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using static buddiesApi.Helpers.SecurityManager;

namespace buddiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CrudController<User, UserService>
    {
        public UserService userService => (service as UserService);
        public UsersController(UserService userService) : base(userService) { }

        [HttpPost("login")]
        public ActionResult<User> Verify(VerifyingUser verifyingUser)
        {
            var user = service.Get(verifyingUser.Email.ToLower());
            if (user == null)
            {
                return new NotFoundResult();
            }
            var verifyingPassword =
                ComputeHash(verifyingUser.Password, ConvertStringSalt(user.Salt));
            if (user.Password == verifyingPassword.HashedSaltedPassword)
            {
                return user;
            }
            return new UnauthorizedResult();
        }

        [HttpPost]
        public override ActionResult<User> Create(User user)
        {
            user.Email = user.Email.ToLower();
            var userWithSameEmail = userService.Get(user.Email);
            if (userWithSameEmail != null) return new ConflictResult();
            user = UserWithSecruredPassword(user);
            service.Create(user);
            return new CreatedResult("users", user);
        }
    }
}
