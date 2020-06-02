using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace buddiesApi.Models
{
    public class UserProfile : IMongoDbDocument
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
        public string Lastname { get; set; }

        [BsonRequired]
        public string City { get; set; }

        [BsonRequired]
        public int BirthDate { get; set; }

        [BsonRequired]
        public string Sex { get; set; }

        public List<string> Languages { get; set; }

        public string Info { get; set; }

        public string RelationshipState { get; set; }

        public List<CategorizedInvolvement> Jobs { get; set; }

        public List<CategorizedInvolvement> Hobbies { get; set; }
    }


    public class CategorizedInvolvement : IMongoDbDocument
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Category { get; set; }

        [BsonRequired]
        public string Title { get; set; }

        public string Place { get; set; }
    }
}
