using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/StopScheduler/{SessionToken}", "GET")]
    [Route("/StopScheduler", "POST")]
    public class StopScheduler : RequestBase, IReturn<StopSchedulerResponse>
    {
    }

    public class StopSchedulerResponse : ResponseBase
    {
    }
}
