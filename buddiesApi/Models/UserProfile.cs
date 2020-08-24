using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace buddiesApi.Models
{
    public class UserProfile : IMongoDbDocument, IUserAvatar
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

        public Image Avatar { get; set; }

        public List<string> Languages { get; set; }

        public string Info { get; set; }

        public string RelationshipState { get; set; }

        public List<CategorizedTag> Jobs { get; set; }

        public List<CategorizedTag> Hobbies { get; set; }
    }

    public interface IUserAvatar {
        public string UserId { get; set; }

        public string Username { get; set; }
     
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public Image Avatar { get; set; }
    }

    public class UserAvatar : IUserAvatar {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Image Avatar { get; set; }
    }
}
