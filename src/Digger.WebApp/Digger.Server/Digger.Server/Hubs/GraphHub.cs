using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Hubs
{
    public class GraphHub : Hub
    {
        public async Task ReceiveRequestDoneGiveNewNode(List<string> userInProject, string newNode, string dataEntity)
        {
            foreach (string user in userInProject)
                Clients.User(user).SendAsync("ReceiveRequestDoneGiveNewNode", newNode, dataEntity);
        }

        public async Task AddToGraphProject(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
        }

        public async Task RemoveToGraphProject(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
        }
    }
}
