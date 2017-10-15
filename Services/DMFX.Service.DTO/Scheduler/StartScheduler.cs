using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/StartScheduler/{SessionToken}", "GET")]
    [Route("/StartScheduler", "POST")]
    public class StartScheduler : RequestBase, IReturn<StartSchedulerResponse>
    {
    }

    public class StartSchedulerResponse : ResponseBase
    {
    }
}
