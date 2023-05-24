using Grpc.Core;
using System.Collections.Concurrent;
using ChatServerApp.Protos;

namespace JustChat.API.Services
{
    public class ChatService : ChatServer.ChatServerBase
    {
        private static readonly Dictionary<string, List<IServerStreamWriter<ChatMessage>>> channels =
    new Dictionary<string, List<IServerStreamWriter<ChatMessage>>>();

        public override async Task<JoinChannelResponse> JoinChannel(JoinChannelRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{request.Username} connected to {request.Channel} channel!");
            string channel = request.Channel;

            if (!channels.ContainsKey(channel))
            {
                channels[channel] = new List<IServerStreamWriter<ChatMessage>>();
            }

            channels[channel].Add(new ChatStreamWriter(context));

            return new JoinChannelResponse { Success = true };
        }

        public override async Task<LeaveChannelResponse> LeaveChannel(LeaveChannelRequest request, ServerCallContext context)
        {
            string channel = request.Channel;

            if (channels.ContainsKey(channel))
            {
                channels[channel].Remove(new ChatStreamWriter(context));

                if (channels[channel].Count == 0)
                {
                    channels.Remove(channel);
                }
            }

            return new LeaveChannelResponse { Success = true };
        }

        public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            string channel = request.Channel;
            string message = request.Message;
            string username = request.Username;

            if (!channels.ContainsKey(channel))
            {
                return new SendMessageResponse { Success = false };
            }

            foreach (var streamWriter in channels[channel])
            {
                await streamWriter.WriteAsync(new ChatMessage { Channel = channel, Username = username, Message = message });
            }

            return new SendMessageResponse { Success = true };
        }

        public override async Task ChatStream(StreamRequest request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            string channel = request.Channel;
            string username = request.Username;

            if (!channels.ContainsKey(channel))
            {
                return;
            }

            var streamWriter = new ChatStreamWriter(context, responseStream);
            channels[channel].Add(streamWriter);

            await Task.Delay(-1, context.CancellationToken);

            channels[channel].Remove(streamWriter);

            if (channels[channel].Count == 0)
            {
                channels.Remove(channel);
            }
        }
    }

    public class ChatStreamWriter : IServerStreamWriter<ChatMessage>
    {
        private ServerCallContext context;
        private IServerStreamWriter<ChatMessage> responseStream;

        public ChatStreamWriter(ServerCallContext context, IServerStreamWriter<ChatMessage> responseStream = null)
        {
            this.context = context;
            this.responseStream = responseStream;
        }

        public WriteOptions? WriteOptions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task WriteAsync(ChatMessage message)
        {
            try
            {
                await responseStream.WriteAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing message: {ex}");
            }
        }
    }
}
