using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    public class AlertSubscriptionProperty
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

    }

    public class AlertSubscription
    {
        public AlertSubscription()
        {
            Properties = new List<AlertSubscriptionProperty>();
        }

        public long ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public long AlertTypeId
        {
            get;
            set;
        }

        public long NotificationTypeId
        {
            get;
            set;
        }

        public DateTime Subscribed
        {
            get;
            set;
        }

        public long StatusId
        {
            get;
            set;
        }

        public IList<AlertSubscriptionProperty> Properties
        {
            get;
            set;
        }
    }
}
