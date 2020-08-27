using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using buddiesApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace buddiesApi.Hubs {
    public class ActivityHub : Hub{

        public async Task AddToActivityUserGroup(string groupName) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task AddToActivityGroups(List<string> groupNames) {
            foreach (string groupName in groupNames) {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task RemoveFromActivityGroup(string groupName) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public Task UpdateActivity(Activity activity) {
            return Clients
                .Group(activity.Id)
                .SendAsync(JsonSerializer.Serialize(activity));
        }

        public Task NewActivityApplication(string activityId, int member) {
            return Clients.Group(activityId).SendAsync(activityId, member);
        }
    }
}
