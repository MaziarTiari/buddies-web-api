using System;
using System.Collections.Generic;
using buddiesApi.Hubs;
using buddiesApi.Models;
using buddiesApi.Models.Chat;
using buddiesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace buddiesApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller<Chat, ChatService> {

        private IHubContext<ChatHub> chatHubContext;

        public ChatController(
            ChatService chatService,
            IHubContext<ChatHub> chatHubContext,
            IHubContext<UserHub> userHubContext
        ): base(chatService) {
            this.chatHubContext = chatHubContext;
        }

        [HttpGet]
        public ActionResult<List<GetChatResponce>> Get() {
            return service.GetUsersMinimumChats(ClientsUserId);
        }

        [HttpGet("UserOnChat/{chatId:length(24)}")]
        public ActionResult UserOnChat(string chatId) {
            var res = service.UserOnChat(chatId, ClientsUserId);
            try {
                if (res.ModifiedCount > 0) {
                    return Ok();
                } else {
                    return NotFound();
                }
            } catch(Exception) {
                return Forbid();
            }
            
        }

        [HttpPost("GetChat")]
        public ActionResult<GetChatResponce> GetChat(GetChatRequest req) {
            GetChatResponce chat = service.GetChatMessagesByIndex(req);
            if (chat == null) {
                return NotFound();
            }
            return Ok(chat);
        }

        [HttpPost("GetMinimumChat")]
        public ActionResult<GetChatResponce> GetMinimumChat(GetMinimumChatRequest req) {
            GetChatResponce chat = service.GetMinimumChat(req, ClientsUserId);
            if (chat == null) {
                return NotFound();
            }
            return Ok(chat);
        }

        [HttpPost("AddMessage/{chatId:length(24)}")]
        public ActionResult AddMessage(string chatId, AddMessageRequest req) {
            Message message = new Message {
                Id = req.Id,
                Body = req.Body,
                AuthorUserId = ClientsUserId,
                SentAt = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
            };
            NewMessage newMessage = new NewMessage {
                Message = message,
                ChatId = chatId
            };
            chatHubContext
                .Clients.Group(chatId).SendAsync("newMessage", newMessage);
            var result = service.AddMessage(chatId, message);
            try {
                if (result.ModifiedCount > 0) {
                    return Ok();
                } else {
                    return NotFound();
                }
            } catch (Exception) {
                return Forbid();
            }
        }

        [HttpPost]
        public ActionResult<GetChatResponce> Create(CreateChatRequest req) {
            var chat = service.CreateChat(req, ClientsUserId);
            if (chat == null) {
                return Forbid();
            }
            foreach(ChatMember member in req.Members) {
                chatHubContext.Clients.Group(member.UserId).SendAsync("newChat", chat);
            }
            return Ok();
        }

        [HttpPost("RecentUpdates")]
        public ActionResult<GetChatResponce> GetRecentUpdates(
                ChatUpdateRequest updateRequest) {
            var res = service.GetChatUpdates(updateRequest);
            if (res == null) {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
