using Grpc.Core;
using System.Collections.Concurrent;
using ChatServerApp.Protos;
using BLL.Interfaces;
using DAL.Entities;
using JustChat.DAL.ViewModel;
using JustChat.BLL.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace JustChat.API.Services
{
    public class ChatService : ChatServer.ChatServerBase
    {
        private ChatControll _chatControll;
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatService(ChatControll chatControll, IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _chatControll = chatControll;
            _messageService = messageService;
            _hubContext = hubContext;
        }

        public override async Task<JoinChannelResponse> JoinChannel(JoinChannelRequest request, ServerCallContext context)  
        {
            Console.WriteLine($"{request.Username} connected!");
            var userName = request.Username;

            var user = new User
            {
                UserName = userName
            };

            var connected = await _chatControll.TryConnectUserAsync(user);

            return new JoinChannelResponse
            {
                Success = true
            };
        }


        public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            var userName = request.Username;
            var user = await _chatControll.GetUserByNameAsync(userName);

            Message newMessage = new Message
            {
                MessageId = Guid.NewGuid(),
                UserName = request.Username,
                Whom = request.Username,
                Text = request.Message,
                PublishDate = DateTime.UtcNow.Millisecond
            };

            var response = new MessageResponse
            {
                message = newMessage,
                rsp =  null
            };

            await _messageService.AddMessageAsync(newMessage);
            await _chatControll.SaveMessageToUsersAsync(user, request.Message);

            await _hubContext.Clients.Group(request.Username).SendAsync("ReceiveGroupMessage", response);

            return new SendMessageResponse { Success = true };
        }

       

        public override async Task<LeaveChannelResponse> LeaveChannel(LeaveChannelRequest request, ServerCallContext context)
        {

            Console.WriteLine($"{request.Username} leave from {request.Channel} channel!");
            return new LeaveChannelResponse { Success = true };
        }

        

        public override async Task ChatStream(StreamRequest request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            var userName = request.Username;
            var user = await _chatControll.GetUserByNameAsync(userName);

            await _chatControll.SubscribeToReceiveMessages(user, responseStream);

            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await _chatControll.BroadcastMessagesAsync(user, context.CancellationToken);
                    await Task.Delay(1000);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                await _chatControll.DisconnectUserAsync(user, responseStream);
            }

        }
    }
}
