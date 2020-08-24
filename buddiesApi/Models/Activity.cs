using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models {
    public class Activity : IMongoDbDocument {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public string Title { get; set; }

        [BsonRequired]
        public string Location { get; set; }

        [BsonRequired]
        public List<string> MemberUserIds { get; set; }

        [BsonRequired]
        public List<string> ApplicantUserIds { get; set; }

        [BsonRequired]
        public int Visibility { get; set; }

        public string Description { get; set; }

        public Image Image { get; set; }

        public long StartDate { get; set; }

        public long EndDate { get; set; }

        public int ApplicationDeadline { get; set; }

        public List<CategorizedTag> Tags { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxMember { get; set; }
    }

    public class ActivityApply {
        public string ApplicantId { get; set; }
        public string ActivityId { get; set; }
    }

    public class OthersActivity : Activity, IUserAvatar {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Image Avatar { get; set; }
    }
}
