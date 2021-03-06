﻿using System;
using buddiesApi.Models;
using MongoDB.Driver;

namespace buddiesApi.Services {
    public class PhotoGalleryService : UserResourceService<PhotoGallery> {

        public PhotoGalleryService(IBuddiesDbContext settings) : base(settings) {
            collection = Database.GetCollection<PhotoGallery>(
                settings.PhotoGalleriesCollectionName);
        }

        public ReplaceOneResult AddImage(string galleryId, ProfileImage image) {
            var gallery = collection.Find(g => g.Id == galleryId).FirstOrDefault();
            gallery.Images.Add(image);
            return collection.ReplaceOne(g => g.Id == galleryId, gallery);
        }
    }
}
