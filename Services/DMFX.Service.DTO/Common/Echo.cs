using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/Echo/{Message}/{SessionToken}", "GET")]
    [Route("/Echo", "POST")]
    public class Echo : RequestBase, IReturn<EchoResponse>
    {
        public string Message
        {
            get; set;
        }
    }

    public class EchoResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public string Message
            {
                get; set;
            }
        }

        public EchoResponse()
        {
            Payload = new ResponsePayload();
        }

        public ResponsePayload Payload
        {
            get;
            set;
        }
    }
}
