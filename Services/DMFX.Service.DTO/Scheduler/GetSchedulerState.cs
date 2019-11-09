using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetSchedulerState/{SessionToken}", "GET")]
    [Route("/GetSchedulerState", "POST")]
    public class GetSchedulerState : RequestBase, IReturn<GetSchedulerStateResponse>
    {
    }

    public class GetSchedulerStateResponse : ResponseBase
    {
        public GetSchedulerStateResponse()
        {
            Payload = new ResponsePayload();
        }

        public ResponsePayload Payload
        {
            get;
            set;
        }

        public class ResponsePayload
        {
            public string State
            {
                get;
                set;
            }
        }
    }
}
