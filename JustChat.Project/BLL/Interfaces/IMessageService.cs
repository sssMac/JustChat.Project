using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IMessageService
    {
        public Task<IEnumerable<Message>> GetChatHistoryAsync();
        public Task<IEnumerable<Message>> GetMessageHistoryAsync(string userName);
        public Task<Message> GetMessageByIdAsync(Guid id);
        public Task<Message> AddMessageAsync(Message message);
        public Task<Message> UpdateMessageAsync(Message message);
        public Task<bool> DeleteMessageAsync(Guid id);
    }
}
