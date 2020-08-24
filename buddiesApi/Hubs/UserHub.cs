using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace buddiesApi.Hubs {
    public class UserHub : Hub{
        public async Task AddToUserGroup(string groupName) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
