﻿using System.Collections.Generic;
using buddiesApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public class PhotoGallery : IMongoDbDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public List<ProfileImage> Images { get; set; }
    }

    public class ProfileImage : Image
    {
        [BsonRequired]
        public bool AsProfile { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public int UploadedDate { get; set; }

        public string Location { get; set; }

        public int Date { get; set; }

        public string Description { get; set; }
    }
}