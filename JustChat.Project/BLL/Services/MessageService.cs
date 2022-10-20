using BLL.Interfaces;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationContext _db;

        public MessageService(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync()
        {
            return await _db.Messages.ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessageHistoryAsync(string userName)
        {
            return await _db.Messages.Where(m => m.UserName == userName).ToListAsync();
        }

        public async Task<Message> GetMessageByIdAsync(Guid id)
        {
            return await _db.Messages.Where(x => x.MessageId == id).FirstOrDefaultAsync();
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            var result = await _db.Messages.AddAsync(message);
            _db.SaveChanges();
            return result.Entity;
        }

        public async Task<Message> UpdateMessageAsync(Message message)
        {
            var result = _db.Messages.Update(message);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> DeleteMessageAsync(Guid id)
        {
            var filteredData = _db.Messages.Where(x => x.MessageId == id).FirstOrDefault();
            var result = _db.Remove(filteredData);
            _db.SaveChanges();
            return result != null ? true : false;
        }

    }
}
