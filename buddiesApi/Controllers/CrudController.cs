using System;
using System.Collections.Generic;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using buddiesApi.Models;

namespace buddiesApi.Controllers
{
    public abstract class CrudController<T,S>
            : ControllerBase where T : IMongoDbDocument where S : IService<T>
    {
        protected readonly S service;

        public CrudController(S service)
        {
            this.service = service;
        }

        [HttpGet]
        public ActionResult<List<T>> Get() => service.Get();

        [HttpGet("{id:length(24)}")]
        public virtual ActionResult<T> Get(string id) => service.Get(id);

        [HttpPost]
        public virtual ActionResult<T> Create(T obj)
        {
            service.Create(obj);
            return new CreatedResult(nameof(obj), obj);
        }

        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            var user = service.Get(id);
            if (user == null) return new NotFoundResult();
            service.Remove(user);
            return new NoContentResult();
        }

        [HttpPut("{id:length(24)}")]
        public virtual ActionResult Update(string id, T newObj)
        {
            var result = service.Update(id, newObj);
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
