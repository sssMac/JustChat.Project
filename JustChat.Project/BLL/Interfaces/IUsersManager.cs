using JustChat.DAL.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.BLL.Interfaces
{
    public interface IUsersManager
    {
        Task<OnlineUser> AddOnlineUser(string userId, string connectionId);
        Task<string> GetConnectionId(string userId);
        Task<string> GetUserId(string connectionId);
        Task<List<OnlineUser>> GetUsers();
        Task<OnlineUser> RemoveOnlineUser(string connectionId);
    }
}
