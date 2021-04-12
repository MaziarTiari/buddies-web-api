using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace buddiesApi.Models
{
    public interface IUserProfile : IUsersDocument, IUserAvatar {
        public string City { get; set; }

        public long BirthDate { get; set; }

        public string Sex { get; set; }

        public List<string> Languages { get; set; }

        public string Info { get; set; }

        public string RelationshipState { get; set; }

        public List<CategorizedTag> Jobs { get; set; }

        public List<CategorizedTag> Hobbies { get; set; }
    }

    public class UserProfile : IUserProfile
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
        public long BirthDate { get; set; }

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

    public class GetUserAvatarsRequest {
        public List<string> UserIds { get; set; }
    }
}
