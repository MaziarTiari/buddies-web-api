using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace buddiesApi.Hubs {
    public class ChatHub : Hub{
        public async Task AddToChatGroup(string groupName) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task AddToChatGroups(List<string> groupNames) {
            foreach(string groupName in groupNames) {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task RemoveFromChatGroup(string groupName) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromChatGroups(List<string> groupNames) {
            foreach (string groupName in groupNames) {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public override Task OnConnectedAsync() {
            Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
