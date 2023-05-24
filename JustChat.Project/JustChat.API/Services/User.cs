using System.Collections.Concurrent;

namespace JustChat.API.Services
{
    public sealed class User : IEquatable<User>
    {
        public string UserName { get; init; }
        private ConcurrentQueue<(string UserName, string Text)> ReceivedMessages { get; init; } = new();

        public void ReceiveMessage(User from, string text)
        {
            ReceivedMessages.Enqueue((from.UserName, text));
        }

        public IEnumerable<(string UserName, string Text)> GetMessages()
        {
            while (ReceivedMessages.TryDequeue(out var message))
            {
                yield return message;
            }
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is User other && Equals(other);
        }

        public bool Equals(User? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return UserName == other.UserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserName);
        }
    }
}
