using BLL.Interfaces;
using DAL.Entities;
using JustChat.BLL.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.BLL.Handlers
{
    public class GetMessageListHandler : IRequestHandler<GetMessageListQuery, List<Message>>
    {
        private readonly IMessageService _dataRepository;

        public GetMessageListHandler(IMessageService dataRepository)
        {
            _dataRepository = dataRepository;
        }
        public async Task<List<Message>> Handle(GetMessageListQuery request, CancellationToken cancellationToken)
        {
            var all = await _dataRepository.GetChatHistoryAsync();
            return all.ToList();
        }
    }
}
