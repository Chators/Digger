using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Hubs
{
    public class ProjectHub : Hub
    {
        public async Task ReceiveInvitationInProject(string userInvitedId, string userAuthorId, string projectId, string authorInvitation, string nameProject)
        {
            await Clients.User(userInvitedId).SendAsync("ReceiveInvitationInProject", authorInvitation, nameProject, userAuthorId, userInvitedId, projectId);
        }

        public async Task ReceiveRequestDone(List<string> userInProject, string projectId, string dataEntity, string nameProject)
        {
            foreach (string user in userInProject)
                Clients.User(user).SendAsync("ReceiveRequestDone", dataEntity, nameProject);
        }

        public async Task ReceiveUserJoinedProject(List<string> userInProject, string projectId, string nameUser, string nameProject)
        {
            foreach (string user in userInProject)
                Clients.User(user).SendAsync("ReceiveUserJoinedProject", nameUser, nameProject);
        }

        public async Task ReceiveUserLeavedProject(List<string> userInProject, string projectId, string nameUser, string nameProject)
        {
            foreach (string user in userInProject)
                Clients.User(user).SendAsync("ReceiveUserLeavedProject", nameUser, nameProject);
        }

        public async Task AddToProject(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
        }

        public async Task RemoveToProject(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
        }
    }
}
