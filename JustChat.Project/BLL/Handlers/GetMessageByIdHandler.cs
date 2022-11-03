using DAL.Entities;
using JustChat.BLL.Queries;
using MediatR;


namespace JustChat.BLL.Handlers
{
    public class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, Message>
    {
        private readonly IMediator _mediator;

        public GetMessageByIdHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<Message> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var messages = await _mediator.Send(new GetMessageListQuery());
            var message = messages.FirstOrDefault(u => u.MessageId == request.id);
            return message;
        }
    }
}
