﻿using System;
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
        public UsersController(UserService userService) : base(userService) { }

        [HttpPost("login")]
        public ActionResult<User> Verify(VerifyingUser verifyingUser)
        {
            var user = (service as UserService).GetByEmail(verifyingUser.Email);
            if (user == null) return new NotFoundResult();
            var verifyingPassword =
                ComputeHash(verifyingUser.Password, Convert.FromBase64String(user.Salt));
            if (user.Password == verifyingPassword.HashedSaltedPassword)
                return user;
            return new UnauthorizedResult();
        }

        [HttpPost]
        public override ActionResult<User> Create(User user)
        {
            var secPass = ComputeHash(user.Password, GenerateSalt());
            user.Password = secPass.HashedSaltedPassword;
            user.Salt = secPass.Salt;
            service.Create(user);
            return new CreatedResult("users", user);
        }
    }
}
