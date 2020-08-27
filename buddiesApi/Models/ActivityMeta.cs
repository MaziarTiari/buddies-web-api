using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models {
    public class ActivityMeta : IMongoDbDocument {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public List<string> FavoriteActivityIds { get; set; }

        [BsonRequired]
        public List<string> HiddenActivityIds { get; set; }
    }
}