using buddiesApi.Middlewares.AuthenticationManager;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static buddiesApi.Helpers.SecurityManager;
using System.Security.Claims;
using System;

namespace buddiesApi.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        readonly IAuthenticationManager authenticationManager;

        readonly UserService userService;

        private string ClientsUserId =>
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public UsersController (
                UserService userService,
                IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
            this.userService = userService;
        }

        [AllowAnonymous] // trivial
        [HttpPost("authenticate")] // akzeptiert HTTP Post Anfragen an .../authenticate
        public ActionResult<User> Authenticate(UserCred userCred) {
            User user = userService.GetUserByEmail(userCred.Email);
            var token = authenticationManager.Authenticate(userCred, user);
            return token == null ? Unauthorized() : Ok(token);
        }

        [HttpGet]
        public ActionResult<UserResponceModel> GetUser() {
            return userService.GetUser(ClientsUserId);
        }

        [AllowAnonymous]
        [HttpPost] // akzeptiert Post Anfragen an api/users
        public ActionResult Create(User user) {
            user.Email = user.Email.ToLower();
            var userWithSameEmail = userService.GetUserByEmail(user.Email);
            if (userWithSameEmail != null) return new ConflictResult();
            user = SecureUsersPassword(user); // kodiere passwort mit salt
            userService.Create(user);
            var token = authenticationManager.CreateBearerToken(user);
            return Ok(token);
        }

        [HttpPut]
        public ActionResult Update(UserInfo userInfo) {            
            var result = userService.UpdateUserInfo(ClientsUserId, userInfo);
            try {
                if (result.MatchedCount > 0) {
                    return new NoContentResult();
                } else {
                    return new NotFoundResult();
                }
            } catch (Exception) {
                return new StatusCodeResult(405);
            }
        }

        [HttpPut("change-password")]
        public ActionResult ChangePassword(ChangePasswordRequestBody body) {
            var user = userService.Get(ClientsUserId);
            if (!authenticationManager.IsCorrectPassword(body.CurrentPassword, user)) {
                return Unauthorized();
            }
            var result = userService.UpdatePassword(user, body.NewPassword);
            try {
                if (result.MatchedCount > 0) {
                    return new NoContentResult();
                } else {
                    return new NotFoundResult();
                }
            } catch (Exception) {
                return new StatusCodeResult(405);
            }
        }
    }
}
