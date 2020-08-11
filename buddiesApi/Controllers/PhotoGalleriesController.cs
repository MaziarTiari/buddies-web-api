using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoGalleriesController
            : CrudController<PhotoGallery, PhotoGalleryService> {

        public PhotoGalleriesController(PhotoGalleryService service) : base(service) { }

        [HttpGet("{userId:length(24)}")]
        public override ActionResult<PhotoGallery> Get(string userId) {
            return base.Get(userId);
        }
    }
}
