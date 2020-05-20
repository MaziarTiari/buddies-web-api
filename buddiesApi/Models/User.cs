using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace buddiesApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Username { get; set; }

        [BsonRequired]
        public string Email { get; set; }

        [BsonRequired]
        public string City { get; set; }

        [BsonRequired]
        public string Password { get; set; }

        [BsonRequired]
        public string Salt { get; set; }

        [BsonRequired]
        public string Phone { get; set; }

        [BsonRequired]
        public int BirthDate { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Sex { get; set; }

        public List<string> Languages { get; set; }

        public string Biography { get; set; }

        public string EducationalInstitute { get; set; }

        public string Company { get; set; }

        public string RelationshipState { get; set; }

        public List<string> Jobs { get; set; }
    }
}
