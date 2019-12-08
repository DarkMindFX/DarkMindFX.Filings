using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/RemoveAccountAlerts/{SubscriptionIds}/{SessionToken}", "GET")]
    [Route("/RemoveAccountAlerts", "POST")]
    public class RemoveAccountAlerts : RequestBase, IReturn<RemoveAccountAlertsResponse>
    {
        public RemoveAccountAlerts()
        {
            SubscriptionIds = new List<long>();
        }

        public string AccountKey
        {
            get;
            set;
        }

        public List<long> SubscriptionIds
        {
            get;
            set;
        }
    }

    public class RemoveAccountAlertsResponse : ResponseBase
    {

    }
}
