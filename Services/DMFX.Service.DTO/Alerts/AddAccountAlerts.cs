using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{

    [Route("/AddAccountAlerts", "POST")]
    public class AddAccountAlerts : RequestBase, IReturn<AddAccountAlertsResponse>
    {
        public AddAccountAlerts()
        {
            Alerts = new List<AlertSubscription>();
        }

        public string AccountKey
        {
            get;
            set;
        }

        public List<AlertSubscription> Alerts
        {
            get;
            set;
        }        
    }

    public class AddAccountAlertsResponse : ResponseBase
    {

    }
}
