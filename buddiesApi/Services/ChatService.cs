using System;
using System.Collections.Generic;
using buddiesApi.Models;
using buddiesApi.Models.Chat;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using MongoDB.Bson;
using static buddiesApi.Helpers.Utils;

namespace buddiesApi.Services {
    public class ChatService : Service<Chat> {

        public ChatService(IBuddiesDbContext settings) : base(settings){
            collection = Database
                .GetCollection<Chat>(settings.ChatCollectionName);
        }

        public UpdateResult UserOnChat(string chatId, string userId) {
            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq(c => c.Id, chatId),
                Builders<Chat>.Filter.ElemMatch(c => c.Members, m => m.UserId == userId));

            var unixNow = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();

            var update = Builders<Chat>.Update
                .Set(c => c.Members.ElementAt(-1).LastTimeOnChat, unixNow);

            var res = collection.UpdateOne(
                filter,
                update);

            return res;
        }

        public UpdateResult AddMessage(
            string chatId,
            Message message
        ) {
            DateTime date = GetDateTimeFromUnixTimeStamp(message.SentAt);

            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq(c => c.Id, chatId),
                Builders<Chat>.Filter.ElemMatch(
                    c => c.Members,
                    m => m.UserId == message.AuthorUserId),
                Builders<Chat>.Filter.ElemMatch(
                    c => c.MessageLists,
                    ml =>
                        ml.Year == date.Year &&
                        ml.Month == date.Month &&
                        ml.Day == date.Day)
                );

            var update = Builders<Chat>
                .Update
                .PushEach(
                    "MessageLists.$[m].Messages",
                    new List<Message> { message },
                    position: 0)
                .Set(c => c.UpdatedAt, message.SentAt);

            var updateOptionsForMatchingMessageList = new UpdateOptions {
                ArrayFilters = new List<ArrayFilterDefinition> {
                    new JsonArrayFilterDefinition<BsonDocument>(
                        $"{{$and:[" +
                            $"{{ \"m.Year\":  {date.Year} }}," +
                            $"{{\"m.Month\": {date.Month } }}," +
                            $"{{\"m.Day\": {date.Day} }}]}}")
                }
            };

            var res = collection.UpdateOne(
                filter,
                update,
                updateOptionsForMatchingMessageList);

            try {
                if (res.ModifiedCount < 1) {
                    filter = Builders<Chat>.Filter.Eq(c => c.Id, chatId);
                    update = Builders<Chat>
                        .Update
                        .PushEach(
                            c => c.MessageLists,
                            new List<MessageList> {
                                new MessageList {
                                    Day = date.Day,
                                    Month = date.Month,
                                    Year = date.Year,
                                    Messages = new List<Message> { message }
                                }
                            },
                            position: 0)
                        .Set(c => c.UpdatedAt, message.SentAt);

                    res = collection.UpdateOne(filter, update);
                        
                }
            } catch(Exception) { }

            return res;
        }

        private DateTime GetDateTimeFromUnixTimeStamp(long unix) {
            DateTime dtDateTime = new DateTime(
                1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            dtDateTime = dtDateTime.AddSeconds(unix);

            return dtDateTime;
        }

        public GetChatResponce GetChatMessagesByIndex(GetChatRequest req) {          
            var projection = Builders<Chat>.Projection
                .Expression(c => new GetChatResponce {
                    Id = c.Id,
                    Members = c.Members,
                    UpdatedAt = c.UpdatedAt,
                    Messages = c.MessageLists.ToList()[req.MessageListIndex].Messages,
                    CreatedAt = c.CreatedAt,
                    MessageListIndex = req.MessageListIndex
                });
            GetChatResponce res = null;
            try {
                res = collection
                    .Find(c => c.Id == req.ChatId)
                    .Project(projection)
                    .FirstOrDefault();
            } catch (Exception) { }

            return res;
        }

        public GetChatResponce GetMinimumChat(GetMinimumChatRequest req, string userId) {
            if (!req.MemberUserIds.Contains(userId)) {
                return null;
            }
            List<FilterDefinition<Chat>> filters = new List<FilterDefinition<Chat>> {
                Builders<Chat>.Filter
                    .Size(c => c.Members, req.MemberUserIds.Count())
            };

            foreach (string id in req.MemberUserIds) {
                filters.Add(
                    Builders<Chat>.Filter.ElemMatch(c =>
                        c.Members, m => m.UserId == id));
            }

            var filter = Builders<Chat>.Filter.And(filters);

            var projection = Builders<Chat>.Projection
                .Expression(c => new GetChatResponce {
                    Id = c.Id,
                    Members = c.Members,
                    UpdatedAt = c.UpdatedAt,
                    Messages = c.MessageLists.First().Messages,
                    CreatedAt = c.CreatedAt,
                    MessageListIndex = 0
                });

            var res = collection
                .Find(filter)
                .Project(projection)
                .FirstOrDefault();

            return res;
        }

        public List<GetChatResponce> GetUsersMinimumChats(string userId) {
            var filter = Builders<Chat>.Filter.ElemMatch(c =>
                c.Members, m => m.UserId == userId);

            var res = collection
                .Find(filter)
                .Project(c => new GetChatResponce {
                    Id = c.Id,
                    Members = c.Members,
                    UpdatedAt = c.UpdatedAt,
                    Messages = c.MessageLists.First().Messages,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            return res;
        }

        public GetChatResponce CreateChat(CreateChatRequest req, string userId) {
            var firstMessageSentAt = GetCurrentUnixTimestamp();

            Message message = new Message {
                Id = req.FirstMessage.Id,
                Body = req.FirstMessage.Body,
                AuthorUserId = userId,
                SentAt = firstMessageSentAt
            };

            var messageDate = GetDateTimeFromUnixTimeStamp(message.SentAt);

            bool membersContainsClientsUserId = false;

            foreach (ChatMember member in req.Members) {
                if (member.UserId == userId) {
                    membersContainsClientsUserId = true;
                    continue;
                }
            }

            if (!membersContainsClientsUserId) {
                return null;
            }

            List<ChatMember> members = req.Members.ToList();

            int authorsMemberIndex = members
                .FindIndex(m => m.UserId == userId);

            members[authorsMemberIndex].LastTimeOnChat = firstMessageSentAt;

            Chat chat = new Chat {
                Members = members,
                UpdatedAt = firstMessageSentAt,
                MessageLists = new List<MessageList> {
                    new MessageList {
                        Day = messageDate.Day,
                        Month = messageDate.Month,
                        Year = messageDate.Year,
                        Messages = new List<Message> {
                            message
                        }
                    }
                },
                CreatedAt = firstMessageSentAt
            };

            Create(chat);

            GetChatResponce res = new GetChatResponce {
                Id = chat.Id,
                CreatedAt = chat.CreatedAt,
                Members = chat.Members,
                Messages = chat.MessageLists.First().Messages,
                UpdatedAt = chat.UpdatedAt
            };

            return res;
        }

        public GetChatResponce GetChatUpdates(ChatUpdateRequest req) {
            var d = GetDateTimeFromUnixTimeStamp(req.LastMessageSentAt);

            return collection
                .Find(c => c.Id == req.ChatId && c.UpdatedAt >= req.LastMessageSentAt)
                .Project(c => new GetChatResponce {
                    Members = c.Members,
                    UpdatedAt = c.UpdatedAt,
                    Messages = c.MessageLists
                        .Where(ml =>
                            ml.Year > d.Year ||
                            (ml.Year == d.Year && ml.Month > d.Month) ||
                            (ml.Year == d.Year && ml.Month == d.Month && ml.Day >= d.Day))
                        .SelectMany(ml =>
                            ml.Messages.Where(m =>
                                (m.SentAt > req.LastMessageSentAt) ||
                                (m.SentAt == req.LastMessageSentAt &&
                                    m.AuthorUserId != req.LastAuthorUserId)))
                        .ToList(),
                    CreatedAt = c.CreatedAt,
                    Id = c.Id
                }).FirstOrDefault();
        }
    }
}
