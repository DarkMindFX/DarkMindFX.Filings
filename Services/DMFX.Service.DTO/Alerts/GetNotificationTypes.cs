using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetNotificationTypes", "GET")]
    [Route("/GetNotificationTypes", "POST")]
    public class GetNotificationTypes : RequestBase, IReturn<GetNotificationTypesResponse>
    {
    }

    public class GetNotificationTypesResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public ResponsePayload()
            {
                Types = new List<NotificationType>();
            }

            public IList<NotificationType> Types
            {
                get; set;
            }
        }

        public GetNotificationTypesResponse()
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
