using System;
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

        [HttpPost("addImage/{galleryId:length(24)}")]
        public ActionResult<Image> AddImage(string galleryId, ProfileImage image) {
            var result = service.AddImage(galleryId, image);
            try {
                if (result.MatchedCount > 0) {
                    return new NoContentResult();
                } else {
                    return new NotFoundResult();
                }
            } catch (Exception) {
                return new StatusCodeResult(405);
            }
        }
    }
}
