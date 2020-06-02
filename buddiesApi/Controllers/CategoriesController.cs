using System;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController: CrudController<Category, CategoryService>
    {
        public CategoriesController(CategoryService service) : base(service)
        {
        }


    }
}
