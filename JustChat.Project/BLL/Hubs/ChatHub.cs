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
            //await LeaveChat();
            var user = await _usersManager.RemoveOnlineUser(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveLeaveUser", user);
        }

        public async Task JoinChat(string userName, bool isAdmin)
        {
            var user = await _usersManager.AddOnlineUser(userName, Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveJoinUser", user);

        }

        public async Task JoinRoom(string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);

        }

        public async Task LeaveRoom()
        {
            var groups = await _usersManager.GetUsers();
             groups.ForEach(async v =>
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, v.Id);

            });

        }

        public async Task LeaveChat()
        {
            var user = await _usersManager.RemoveOnlineUser(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveLeaveUser", user);

        }

        public async Task GetMySelf(string userName)
        {
            var user = (await _usersManager.GetUsers()).FirstOrDefault(u => u.Id == userName);
            await Clients.Caller.SendAsync("ReceiveMyUser", user);

        }

    }
}
