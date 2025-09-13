using Auto.Application._Shared;
using Auto.Domain.ViewModels.LED;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Application.LED
{
    public record SendLedStatusCommand(LedCommand Command, string Topic) : IRequest<OperationResult>;
}
