using System;
using System.Collections.Generic;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Controllers
{
    public abstract class CrudController<T,S> : ControllerBase where T : IMongoDbDocument
    {
        protected readonly Service<T> service;

        public CrudController(Service<T> service)
        {
            this.service = service;
        }

        [HttpGet]
        public ActionResult<List<T>> Get() => service.Get();

        [HttpGet("{id:length(24)}")]
        public ActionResult<T> Get(string id) => service.Get(id);

        [HttpPost]
        public virtual ActionResult<T> Create(T obj)
        {
            service.Create(obj);
            return new CreatedResult(nameof(obj)+"s", obj);
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
            try
            {
                var result = service.Update(id, newObj);
                if(result.IsModifiedCountAvailable && result.ModifiedCount > 0)
                {
                    return new NoContentResult();
                } else
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
