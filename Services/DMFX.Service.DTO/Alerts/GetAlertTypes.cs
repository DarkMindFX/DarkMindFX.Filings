using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetAlertsTypes", "GET")]
    [Route("/GetAlertsTypes", "POST")]
    public class GetAlertsTypes : RequestBase, IReturn<GetAlertsTypesResponse>
    {
    }

    public class GetAlertsTypesResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public ResponsePayload()
            {
                Types = new List<AlertType>();
            }

            public IList<AlertType> Types
            {
                get; set;
            }
        }

        public GetAlertsTypesResponse()
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
