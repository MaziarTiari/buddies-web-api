using System;
using System.Collections.Generic;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using buddiesApi.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace buddiesApi.Controllers
{
    public abstract class Controller<T,S>
            : ControllerBase where T : IMongoDbDocument where S : IService<T>
    {
        protected readonly S service;
        protected string ClientsUserId =>
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public Controller(S service)
        {
            this.service = service;
        }

        [HttpGet("all")]
        public virtual ActionResult<List<T>> GetAll() => service.Get();

        [HttpGet("{id:length(24)}")]
        public virtual ActionResult<T> Get(string id) {
            return service.Get(id);
        }

        [HttpPost]
        public virtual ActionResult Create(T obj)
        {
            service.Create(obj);
            return new CreatedResult(nameof(obj), obj);
        }

        [HttpDelete("{id:length(24)}")]
        public virtual ActionResult Delete(string id)
        {
            var user = service.Get(id);
            if (user == null) return new NotFoundResult();
            service.Remove(user);
            return new NoContentResult();
        }

        [HttpPut("{id:length(24)}")]
        public virtual ActionResult Replace(string id, T newObj)
        {
            var result = service.Replace(id, newObj);
            try
            {
                if(result.MatchedCount > 0)
                {
                    return new NoContentResult();
                }
                else
                {
                    return new NotFoundResult();
                }
            } catch(Exception)
            {
                return new StatusCodeResult(405);
            }
        }
    }
}
