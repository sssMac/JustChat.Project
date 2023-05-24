using ChatServerApp.Protos;
using DAL.Entities;
using Grpc.Core;
using JustChat.BLL.Hubs;
using JustChat.BLL.Interfaces;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace JustChat.API.Services
{
    public class ChatControll
    {
        private static readonly ConcurrentDictionary<User, List<IServerStreamWriter<ChatMessage>>> Users = new();
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUsersManager _usersManager;

        public ChatControll(IHubContext<ChatHub> hubContext, IUsersManager usersManager)
        {
            _hubContext = hubContext;
            _usersManager = usersManager;
        }

        public Task<User> GetUserByNameAsync(string userName)
        {
            var user = Users.Keys.First(u => u.UserName == userName);

            return Task.FromResult(user);
        }

        public async Task<bool> TryConnectUserAsync(User user)
        {
            var added = Users.TryAdd(user, new List<IServerStreamWriter<ChatMessage>>());

            var guid = Guid.NewGuid().ToString();
            var userr = await _usersManager.AddOnlineUser(user.UserName, guid);

            Console.WriteLine($"try add to signalR {userr.Id} - {userr.ConnectionId}");
            await _hubContext.Clients.All.SendAsync("ReceiveJoinUser", userr);
            await _hubContext.Groups.AddToGroupAsync(guid, user.UserName);
            await _usersManager.AddToGroup(user.UserName, user.UserName);

            return added;
        }

        public async Task DisconnectUserAsync(User user, IServerStreamWriter<ChatMessage> serverStreamWriter)
        {
            Users[user].Remove(serverStreamWriter);

            if (!Users[user].Any())
            {
                Users.TryRemove(user, out _);
                var userr = (await _usersManager.GetUsers()).FirstOrDefault(u => u.Id == user.UserName);
                var userrr = await _usersManager.RemoveOnlineUser(userr.ConnectionId);
                await _usersManager.RemoveFromGroup(user.UserName, user.UserName);

            }

        }

        public Task SubscribeToReceiveMessages(User user, IServerStreamWriter<ChatMessage> responseWriter)
        {
            Users[user].Add(responseWriter);

            return Task.FromResult(true);
        }

        public Task SaveMessageToUsersAsync(User user, string text)
        {
            Console.WriteLine($"Сохранение сообщени для  {user.UserName}");
            foreach (var pair in Users)
            {
                pair.Key.ReceiveMessage(user, text);
            }

            return Task.CompletedTask;
        }

        public async Task BroadcastMessagesAsync(User user, CancellationToken cancellationToken)
        {
            var streamWriters = Users[user];

            var messages = user.GetMessages();

            foreach (var message in messages)
            {
                var response = new ChatMessage
                {
                    Username = message.UserName,
                    Message = message.Text
                };
                Console.WriteLine($"{message.UserName} получает {message.Text}");
                foreach (var streamWriter in streamWriters)
                {
                    await streamWriter.WriteAsync(response, cancellationToken);
                }
            }
        }
    }

}

