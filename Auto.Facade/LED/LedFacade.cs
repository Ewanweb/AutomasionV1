using Auto.Application._Shared;
using Auto.Application.LED;
using Auto.Domain.ViewModels.LED;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Facade.LED
{
    public class LedFacade : ILedFacade
    {
        private readonly IMediator _mediator;

        public LedFacade(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<OperationResult> SendLedStatus(LedCommand command, string topic)
        {
            return await _mediator.Send(new SendLedStatusCommand(command, topic));
        }

    }
}
