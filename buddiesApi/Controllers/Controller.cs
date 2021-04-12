using System;
using System.Security.Claims;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers {
    public abstract class Controller<T, S>
            : ControllerBase where T : IMongoDbDocument where S : IService<T> {
        protected readonly S service;
        protected string ClientsUserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public Controller(S service) {
            this.service = service;
        }
    }
}
