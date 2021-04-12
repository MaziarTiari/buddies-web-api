using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers {

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : CRUDController<Category, CategoryService> {

        public CategoriesController(CategoryService service) : base(service) { }

        [HttpGet("{title}")]
        public override ActionResult<Category> Get(string title) {
            return (service as CategoryService).Get(title);
        }
    }
}
