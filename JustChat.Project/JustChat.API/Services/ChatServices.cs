﻿using ChatServerApp.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace JustChat.API.Services
{
    public class ChatService : ChatServer.ChatServerBase
    {
        private readonly ILogger<ChatService> _logger;
        private readonly ChatRoomService _chatRoomService;

        public ChatService(ChatRoomService chatRoomService, ILogger<ChatService> logger)
        {
            _chatRoomService = chatRoomService;
            _logger = logger;
        }

        public override async Task HandleCommunication(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            var userName = string.Empty;
            var chatRoomId = string.Empty;

            while (true)
            {
                //Read a message from the client.
                var clientMessage = await _chatRoomService.ReadMessageWithTimeoutAsync(requestStream, Timeout.InfiniteTimeSpan);
                switch (clientMessage.ContentCase)
                {
                    case ClientMessage.ContentOneofCase.Login:

                        var loginMessage = clientMessage.Login;
                        //get username and chatRoom Id from clientMessage.
                        chatRoomId = loginMessage.ChatRoomId;
                        userName = loginMessage.UserName;

                        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(chatRoomId))
                        {
                            //Send a login Failure message.
                            var failureMessage = new ServerMessage
                            {
                                LoginFailure = new ServerMessageLoginFailure { Reason = "Invalid username" }
                            };

                            _logger.LogInformation(failureMessage.LoginFailure.Reason);
                            await responseStream.WriteAsync(failureMessage);

                            return;
                        }

                        //Send login succes message to client
                        var successMessage = new ServerMessage { LoginSuccess = new ServerMessageLoginSuccess() };

                        _logger.LogInformation($"{loginMessage.UserName} login success!");
                        await responseStream.WriteAsync(successMessage);

                        //Add client to chat room.
                        await _chatRoomService.AddClientToChatRoom(chatRoomId, new ChatClient
                        {
                            StreamWriter = responseStream,
                            UserName = userName
                        });

                        break;

                    case ClientMessage.ContentOneofCase.Chat:

                        var chatMessage = clientMessage.Chat;

                        if (userName is not null && chatRoomId is not null)
                        {
                            //broad cast the message to the room
                            _logger.LogInformation($"{chatRoomId} : {userName} send : {chatMessage.Text}!");
                            await _chatRoomService.BroadcastMessageToChatRoom(chatRoomId, userName, chatMessage.Text);
                        }

                        break;
                }
            }
        }
    }
}
