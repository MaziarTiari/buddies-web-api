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
    }
}
