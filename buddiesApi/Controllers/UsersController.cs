using buddiesApi.Middlewares.AuthenticationManager;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static buddiesApi.Helpers.SecurityManager;

namespace buddiesApi.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CrudController<User, UserService> {
        readonly IAuthenticationManager authenticationManager;

        public UsersController (
                UserService userService,
                IAuthenticationManager authenticationManager): base(userService)
        {
            this.authenticationManager = authenticationManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<User> SignIn(UserCred userCred) {
            User user = service.Get(userCred.Email);
            var token = authenticationManager.Authenticate(userCred, user);
            return token == null ? Unauthorized() : Ok(token);
        }

        [HttpPost]
        public override ActionResult<User> Create(User user) {
            user.Email = user.Email.ToLower();
            var userWithSameEmail = service.Get(user.Email);
            if (userWithSameEmail != null) return new ConflictResult();
            user = UserWithSecruredPassword(user);
            service.Create(user);
            return new CreatedResult("users", user);
        }
    }
}
