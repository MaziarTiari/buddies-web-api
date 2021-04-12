using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace buddiesApi.Models.Chat {

    public class GetChatRequest {
        [Required]
        [MinLength(24, ErrorMessage = "String must have mimimum length of 24 digit")]
        public string ChatId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int MessageListIndex { get; set; }
    }

    public class AddMessageRequest {
        [Required]
        [MinLength(25, ErrorMessage = "String must have mimimum length of 25 digit")]
        public string Id { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "String must have mimimum length of 1 digit")]
        public string Body { get; set; }
    }

    public class Message : AddMessageRequest {
        public string AuthorUserId { get; set; }
        public long SentAt { get; set; }
    }

    public class NewMessage {
        public string ChatId { get; set; }
        public Message Message { get; set; }
    }

    public class CreateChatRequest {
        [Required]
        public AddMessageRequest FirstMessage { get; set; }

        [MinLength(2, ErrorMessage = "Must be an array with minimum length of 2")]
        [Required]
        public IEnumerable<ChatMember> Members { get; set; }
    }

    public class GetMinimumChatRequest {
        [Required]
        [MinLength(2, ErrorMessage = "Must be an array with minimum length of 2")]
        public IEnumerable<string> MemberUserIds { get; set; }
    }

    public class ChatUpdateRequest {
        [Required]
        [MinLength(24, ErrorMessage = "String must have mimimum length of 24 digit")]
        public string ChatId { get; set; }

        [Required]
        [MinLength(24, ErrorMessage = "String must have mimimum length of 24 digit")]
        public string LastAuthorUserId { get; set; }

        [Required]
        public long LastMessageSentAt { get; set; }
    }

    public class GetChatResponce : IChatMeta {
        public string Id { get; set; }
        public IEnumerable<ChatMember> Members { get; set; }
        public long UpdatedAt { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public long CreatedAt { get; set; }
        public int MessageListIndex { get; set; }
    }

    public interface IChatMeta {
        public string Id { get; set; }
        public IEnumerable<ChatMember> Members { get; set; }
        public long UpdatedAt { get; set; }
        public long CreatedAt { get; set; }
    }

    public class Chat : IMongoDbDocument, IChatMeta {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public IEnumerable<ChatMember> Members { get; set; }

        [BsonRequired]
        public long UpdatedAt { get; set; }

        [BsonRequired]
        public IEnumerable<MessageList> MessageLists { get; set; }

        [BsonRequired]
        public long CreatedAt { get; set; }
    }

    public class ChatMember {
        [Required]
        [MinLength(24,ErrorMessage = "String must have mimimum length of 24 digit")]
        public string UserId { get; set; }

        [Required]
        public long LastTimeOnChat { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
}

    public class MessageList {
        [BsonRequired]
        [Range(2021, int.MaxValue)]
        public int Year { get; set; }

        [BsonRequired]
        [Range(1, 12)]
        public int Month { get; set; }

        [BsonRequired]
        [Range(1, 31)]
        public int Day { get; set; }

        [BsonRequired]
        public IEnumerable<Message> Messages { get; set; }
    }
}
