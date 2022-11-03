using BLL.Interfaces;
using DAL.Entities;
using JustChat.BLL.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.BLL.Handlers
{
    public class AddMessageHandler : IRequestHandler<AddMessageCommand, Message>
    {
        private readonly IMessageService _dataRepository;

        public AddMessageHandler(IMessageService dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<Message> Handle(AddMessageCommand request, CancellationToken cancellationToken)
        {
            return await _dataRepository.AddMessageAsync(request.model);
        }
    }
}
