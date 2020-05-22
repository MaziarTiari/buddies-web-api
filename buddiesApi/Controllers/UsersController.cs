using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using static buddiesApi.Helpers.SecurityHelper;

namespace buddiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserService userService;

        public UsersController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult<List<User>> Get() => userService.Get();

        [HttpGet("{id:length(24)}")]
        public ActionResult<User> Get(string id) => userService.Get(id);

        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            SecuredPassword sp = ComputeHash(user.Password, GenerateSalt());
            user.Password = sp.HashedSaltedPassword;
            user.Salt = sp.Salt;
            if (EmailExists(user.Email)) return new ConflictResult();
            userService.Create(user);
            return new CreatedResult("users", user);
        }

        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            var user = userService.Get(id);
            if (user == null) return new NotFoundResult();
            userService.Remove(user);
            return new NoContentResult();
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, User userIn)
        {
            var user = userService.Get(id);
            if (user == null) return new NotFoundResult();
            userService.Update(id, userIn);
            return new NoContentResult();
        }

        [HttpPost("login")]
        public ActionResult<User> Verify(VerifyingUser verifyingUser)
        {
            var user = userService.GetByEmail(verifyingUser.Email);
            if (user == null) return new NotFoundResult();
            var verifyingPassword =
                ComputeHash(verifyingUser.Password, Convert.FromBase64String(user.Salt));
            if (user.Password == verifyingPassword.HashedSaltedPassword)
                return user;
            return new UnauthorizedResult();
        }

        private bool EmailExists(string email)
        {
            List<User> users = userService.Get();
            return users.Find(u => u.Email == email) != null ? true : false;
        }
    }
}
