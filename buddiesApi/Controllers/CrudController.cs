using System;
using System.Collections.Generic;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers
{
    public abstract class CrudController<T,S> : ControllerBase
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
        public ActionResult Update(string id, T newObj)
        {
            var user = service.Get(id);
            if (user == null) return new NotFoundResult();
            service.Update(id, newObj);
            return new NoContentResult();
        }
    }
}
