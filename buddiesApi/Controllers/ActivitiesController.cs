using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : CrudController<Activity, ActivityService> {

        public ActivitiesController(ActivityService service) : base(service) { }

        [HttpGet("{userId:length(24)}")]
        public override ActionResult<Activity> Get(string userId) {
            return base.Get(userId);
        }
    }
}
