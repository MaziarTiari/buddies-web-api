using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public class UserAvatar : IMongoDbDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public string Username { get; set; }

        [BsonRequired]
        public string Firstname { get; set; }

        [BsonRequired]
        public string Surname { get; set; }

        [BsonRequired]
        public Image Avatar { get; set; }
    }
}
