using buddiesApi.Middlewares.AuthenticationManager;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static buddiesApi.Helpers.SecurityManager;
using static buddiesApi.Helpers.Utils;
using System.Security.Claims;
using System;

namespace buddiesApi.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller {
        private const string verificationErrorHtml = "<h1 style=\"color: #7A1D2F; font-size: calc(24px + 5vw); text-align: center; margin-top: 30vh\">Falsche Anfrage!</h1>";
        private const string verificationSuccessHtml = "<h1 style=\"color: #1D7A57; font-size: calc(24px + 5vw); text-align: center; margin-top: 30vh\">Vielen Dank!</h1>";

        private string NewVerificationCode => RandomString(40);

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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate(UserCred userCred) {
            User user = userService.GetUserByEmail(userCred.Email);
            if (user.IdentityVerificationCode != null) {
                return Forbid();
            }
            else if (user.SignInRetries == 3) {
                string verificationCode = NewVerificationCode;
                user.IdentityVerificationCode = verificationCode;
                userService.Update(user.Id, user);

                var link = GetVerificationLink(
                    VerificationType.Identity,
                    user.Email,
                    verificationCode);

                authenticationManager.SendVerificationEmail(
                    VerificationType.Identity,
                    link,
                    user.Email);

                return Forbid();
            }
            var token = authenticationManager.Authenticate(userCred, user);
            if (token == null) {
                user.SignInRetries += 1;
                userService.Update(user.Id, user);
                return Unauthorized();
            }
            if (!user.EmailConfirmed) {
                return NoContent();
            }
            else if (user.SignInRetries > 0) {
                user.SignInRetries = 0;
                userService.Update(user.Id, user);
            }
            return Ok(token);
        }

        private string GetVerificationLink(
            VerificationType type,
            string email,
            string code
        ) {
            string endpoint = type == VerificationType.Email
                ? "ConfirmEmail/" : "VerifyIdentity/";

            return Request.Scheme + "://" + Request.Host +
                "/api/Users/" + endpoint + email + "/" + code;
        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail/{email}/{code}")]
        public ContentResult ConfirmEmail(string email, string code) {
            User user = userService.GetUserByEmail(email);
            if (user == null || user.EmailVerificationCode != code) {
                return base.Content(verificationErrorHtml, "text/html"); ;
            }
            user.EmailConfirmed = true;
            user.EmailVerificationCode = null;
            userService.Update(user.Id, user);
            return base.Content(verificationSuccessHtml, "text/html");
        }

        [AllowAnonymous]
        [HttpGet("VerifyIdentity/{email}/{code}")]
        public ContentResult VerifyIdentity(string email, string code) {
            User user = userService.GetUserByEmail(email);
            if (user == null || user.IdentityVerificationCode != code) {
                return base.Content(verificationErrorHtml, "text/html"); ;
            }
            user.SignInRetries = 0;
            user.IdentityVerificationCode = null;
            userService.Update(user.Id, user);
            return base.Content(verificationSuccessHtml, "text/html");
        }

        [HttpGet]
        public ActionResult<UserResponce> GetUser() {
            var user = userService.GetUser(ClientsUserId);
            if (user == null) {
                return NotFound();
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Create(NewUser req) {
            req.Email = req.Email.ToLower();
            var userWithSameEmail = userService.GetUserByEmail(req.Email);
            if (userWithSameEmail != null) {
                return new ConflictResult();
            }
            string verificationCode = NewVerificationCode;
            User user = new User {
                Email = req.Email,
                Password = req.Password,
                Phone = req.Phone,
                EmailConfirmed = false,
                EmailVerificationCode = verificationCode,
            };
            user = EncodeUsersPassword(user);
            userService.Create(user);
            authenticationManager.SendVerificationEmail(
                VerificationType.Email,
                GetVerificationLink(VerificationType.Email, user.Email, verificationCode),
                user.Email);
            return NoContent();
        }

        [HttpPut]
        public ActionResult Update(UserInfo userInfo) {            
            var result = userService.UpdateUserInfo(ClientsUserId, userInfo);
            return UpdateActionResult(result);
        }

        [HttpPut("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordRequestBody body) {
            var user = userService.Get(ClientsUserId);
            if (!authenticationManager.IsCorrectPassword(body.CurrentPassword, user)) {
                return Unauthorized();
            }
            var result = userService.UpdatePassword(user, body.NewPassword);
            return UpdateActionResult(result);
        }
    }
}
