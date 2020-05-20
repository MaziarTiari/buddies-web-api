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

        [HttpPost("/create")]
        public ActionResult<User> Create(User user)
        {
            SecuredPassword sp = ComputeHash(user.Password, GenerateSalt());
            user.Password = sp.HashedSaltedPassword;
            user.Salt = sp.Salt;
            userService.Create(user);
            if (EmailExists(user.Email)) return new ConflictResult();
            return new CreatedResult("users", user);
        }

        [HttpPost]


        private bool EmailExists(string email)
        {
            List<User> users = userService.Get();
            return users.Find(u => u.Email == email) != null ? true : false;
        }
    }
}
