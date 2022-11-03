using DAL.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.BLL.Queries
{
    public record GetMessageByIdQuery(Guid id) : IRequest<Message>;

}
