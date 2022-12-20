using JustChat.BLL.Interfaces;
using JustChat.DAL.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.BLL.Services
{
    public class UsersManager : IUsersManager
    {
        private List<OnlineUser> _onlineUsers;
        private List<ChatGroup> _groups;

        public UsersManager()
        {
            _onlineUsers = new List<OnlineUser>();
            _groups = new List<ChatGroup>();
        }

        public async Task<OnlineUser> AddOnlineUser(string userId, string connectionId)
        {
            var usr = _onlineUsers.Where(usr => usr.Id == userId).FirstOrDefault();
            if (usr != null)
            {
                _onlineUsers.Remove(usr);
            }

            var newUsr = new OnlineUser
            {
                Id = userId,
                ConnectionId = connectionId,
                TimeJoined = DateTime.Now
            };

            _onlineUsers.Add(newUsr);
            return newUsr;
        }

        public async Task<OnlineUser> RemoveOnlineUser(string connectionId)
        {
            var user = _onlineUsers.Where(usr => connectionId == usr.ConnectionId).FirstOrDefault();
            _onlineUsers.Remove(user);
            return user;
        }

        public Task<string> GetConnectionId(string userId)
        {
            return Task.Run(() =>
            {
                return _onlineUsers
                .Where(usr => userId == usr.Id)
                .Select(usr => usr.ConnectionId)
                .FirstOrDefault();
            });
        }

        public Task<string> GetUserId(string connectionId)
        {
            return Task.Run(() =>
            {
                return _onlineUsers
                .Where(usr => connectionId == usr.ConnectionId)
                .Select(usr => usr.Id)
                .FirstOrDefault();
            });
        }

        public Task<List<OnlineUser>> GetUsers()
        {
            return Task.Run(() =>
            {
                return _onlineUsers;

            });
        }

        public async Task<bool> AddToGroup(string userName, string groupName)
        {
            _groups.Add(new ChatGroup
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Group = groupName
            });
            return true;
        }

        public async Task<bool> RemoveFromGroup(string userName, string groupName)
        {
            _groups.Remove(_groups.FirstOrDefault(u => u.UserName == userName && u.Group == groupName));
            return true;
        }
        public async Task<List<ChatGroup>> GetGroups(string userName)
        {
            return _groups.Where(u => u.UserName == userName).ToList();

        }
    }
}
