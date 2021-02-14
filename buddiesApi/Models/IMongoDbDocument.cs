using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models {
    public interface IMongoDbDocument {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

    public interface IUsersDocument : IMongoDbDocument {
        public string UserId { get; set; }
    }
}
