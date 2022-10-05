using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IMessageService
    {
        public IEnumerable<Message> GetChatHistory();
        public IEnumerable<Message> GetMessageHistory(string userName);
        public Message GetMessageById(Guid id);
        public Task<Message> AddMessage(Message message);
        public Message UpdateMessage(Message message);
        public bool DeleteMessage(Guid id);
    }
}
