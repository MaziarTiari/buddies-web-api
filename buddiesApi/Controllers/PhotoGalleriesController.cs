using System;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace buddiesApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoGalleriesController
            : Controller<PhotoGallery, PhotoGalleryService> {

        public PhotoGalleriesController(PhotoGalleryService service) : base(service) { }

        [HttpGet]
        public ActionResult<PhotoGallery> GetUsersProfile() {
            return Get(ClientsUserId);
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
