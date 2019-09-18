using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace rgnl_server.Hubs
{
    public class PostHub : Hub
    {
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}