using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{

    [Route("/UpdateAccountAlerts", "POST")]
    public class UpdateAccountAlerts : RequestBase, IReturn<UpdateAccountAlertsResponse>
    {
        public UpdateAccountAlerts()
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

    public class UpdateAccountAlertsResponse : ResponseBase
    {

    }
}
