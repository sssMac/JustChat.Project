using DAL.Entities;
using JustChat.BLL.Interfaces;
using JustChat.DAL.ViewModel;
using Microsoft.AspNetCore.SignalR;

namespace JustChat.API.Hubs
{
    public class ChatHub : Hub
    {

        public async Task SendMessage(MessageRequest mess)
        {

            await Clients.All.SendAsync("ReceiveMessage", mess);

        }

     
    }
}
