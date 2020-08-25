using System;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services {
    public class PhotoGalleryService : Service<PhotoGallery> {

        public PhotoGalleryService(IBuddiesDbContext settings) : base(settings) {
            collection = GetDatabase.GetCollection<PhotoGallery>(
                settings.PhotoGalleriesCollectionName);
        }

        public override PhotoGallery Get(string userId) {
            return collection.Find(o => o.UserId == userId).FirstOrDefault();
        }

        public ReplaceOneResult AddImage(string galleryId, ProfileImage image) {
            var gallery = collection.Find(g => g.Id == galleryId).FirstOrDefault();
            gallery.Images.Add(image);
            return collection.ReplaceOne(g => g.Id == galleryId, gallery);
        }
    }
}
