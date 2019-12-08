using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetAccountAlerts/{AccountKey}/{SessionToken}", "GET")]
    [Route("/GetAccountAlerts", "POST")]
    public class GetAccountAlerts : RequestBase, IReturn<GetAccountAlertsResponse>
    {
        public string AccountKey
        {
            get;
            set;
        }
    }

    public class GetAccountAlertsResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public ResponsePayload()
            {
                Alerts = new List<AlertSubscription>();
            }

            public string AccountKey
            {
                get;
                set;
            }

            public IList<AlertSubscription> Alerts
            {
                get;
                set;
            }
        }

        public GetAccountAlertsResponse()
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
