using DAL.Entities;
using MediatR;

namespace JustChat.BLL.Commands
{
    public record AddMessageCommand(Message model) : IRequest<Message>;
}
