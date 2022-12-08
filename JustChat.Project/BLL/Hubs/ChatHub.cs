using DAL.Entities;
using JustChat.BLL.Interfaces;
using JustChat.DAL.ViewModel;
using Microsoft.AspNetCore.SignalR;

namespace JustChat.BLL.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUsersManager _usersManager;

        public ChatHub(IUsersManager usersManager)
        {
            _usersManager = usersManager;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await LeaveChat();
        }

        public async Task JoinChat(string userName)
        {
            var user = await _usersManager.AddOnlineUser(userName, Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveJoinUser", user);

        }

        public async Task LeaveChat()
        {
            var user = await _usersManager.RemoveOnlineUser(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveLeaveUser", user);

        }

    }
}
