using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreakerInfrastructure
{
    public interface IExecutionResult
    {
        IBrakerResponse Execute(IBreakerProxy breaker);
    }
}
