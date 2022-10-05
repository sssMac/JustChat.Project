using BLL.Interfaces;
using DAL.Data;
using DAL.Entities;

namespace BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationContext _db;

        public MessageService(ApplicationContext db)
        {
            _db = db;
        }

        public IEnumerable<Message> GetChatHistory()
        {
            return _db.Messages.ToList();
        }

        public IEnumerable<Message> GetMessageHistory(string userName)
        {
            return _db.Messages.Where(m => m.UserName == userName).ToList();
        }

        public Message GetMessageById(Guid id)
        {
            return _db.Messages.Where(x => x.MessageId == id).FirstOrDefault();
        }

        public async Task<Message> AddMessage(Message message)
        {
            var result = await _db.Messages.AddAsync(message);
            _db.SaveChanges();
            return result.Entity;
        }

        public Message UpdateMessage(Message message)
        {
            var result = _db.Messages.Update(message);
            _db.SaveChanges();
            return result.Entity;
        }

        public bool DeleteMessage(Guid id)
        {
            var filteredData = _db.Messages.Where(x => x.MessageId == id).FirstOrDefault();
            var result = _db.Remove(filteredData);
            _db.SaveChanges();
            return result != null ? true : false;
        }

    }
}
