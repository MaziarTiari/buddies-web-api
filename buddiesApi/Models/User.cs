using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public class User : IUserInfo, IMongoDbDocument {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Password { get; set; }

        [BsonRequired]
        public string Salt { get; set; }

        [BsonRequired]
        public string Phone { get; set; }

        [BsonRequired]
        public string Email { get; set; }

    }

    public class UserCred {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public interface IUserInfo {
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class UserInfo : IUserInfo {
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class UserResponceModel: UserInfo {
        public string Id { get; set; }
    }


    public class ChangePasswordRequestBody {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
