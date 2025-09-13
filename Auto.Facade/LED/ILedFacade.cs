using Auto.Application._Shared;
using Auto.Domain.ViewModels.LED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Facade.LED
{
    public interface ILedFacade
    {
        Task<OperationResult> SendLedStatus(LedCommand command, string topic);
    }
}
