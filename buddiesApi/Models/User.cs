using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models
{
    public interface IWithValidPassword {
        [ValidPassword]
        public string Password { get; set; }
    }
    public class CreateUserRequest : IWithValidPassword {
        [Required]
        [ValidPassword]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

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

        [BsonRequired]
        public bool EmailConfirmed { get; set; } = false;

        [BsonRequired]
        public string EmailVerificationCode { get; set; }

        [BsonRequired]
        public string IdentityVerificationCode { get; set; } = null;

        [BsonRequired]
        public int SignInRetries { get; set; } = 0;
    }

    public class NewUser : UserInfo {
        [Required]
        public string Password { get; set; }
    }

    public class UserCred {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public interface IUserInfo {
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class UserInfo : IUserInfo {
        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class UserResponce: UserInfo {
        public string Id { get; set; }
    }


    public class ChangePasswordRequestBody {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
