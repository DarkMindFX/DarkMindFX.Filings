using DMFX.AlertsInterfaces;
using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.AlertsDAL.MSSQL
{
    public class AlertsDalInitParams : IAlertsDalInitParams
    {
        public AlertsDalInitParams()
        {
            Parameters = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Parameters
        {
            get;
            set;
        }
    }

    public class AlertsDalGetAlertTypesParams : IAlertsDalGetAlertTypesParams
    {
    }

    public class AlertsDalGetAlertTypesResult : ResultBase, IAlertsDalGetAlertTypesResult
    {
        public AlertsDalGetAlertTypesResult()
        {
            Types = new List<AlertType>();
        }

        public IList<AlertType> Types
        {
            get;
            private set;
        }
    }

    public class AlertsDalGetAlertNotificationTypesParams : IAlertsDalGetAlertNotificationTypesParams
    {
    }

    public class AlertsDalGetAlertNotificationTypesResult : ResultBase, IAlertsDalGetAlertNotificationTypesResult
    {
        public AlertsDalGetAlertNotificationTypesResult()
        {
            Types = new List<AlertNotificationType>();
        }

        public IList<AlertNotificationType> Types
        {
            get;
            set;
        }
    }

    public class AlertsDalAddAccountSubscrParams : IAlertsDalAddAccountSubscrParams
    {
        public Subscription SubscriptonDetails
        {
            get;
            set;
        }
    }

    public class AlertsDalAddAccountSubscrResult : ResultBase, IAlertsDalAddAccountSubscrResult
    {
    }

    public class AlertsDalUpdateAccountSubscrParams : IAlertsDalUpdateAccountSubscrParams
    {
        public Subscription SubscriptonDetails
        {
            get;
            set;
        }
    }

    public class AlertsDalUpdateAccountSubscrResult : ResultBase, IAlertsDalUpdateAccountSubscrResult
    {
    }

    public class AlertsDalRemoveAccountSubscrParams : IAlertsDalRemoveAccountSubscrParams
    {
        public AlertsDalRemoveAccountSubscrParams()
        {
            SubscriptionIds = new List<long>();
        }

        public IList<long> SubscriptionIds
        {
            get;
            set;
        }
    }

    public class AlertsDalRemoveAccountSubscrResult : ResultBase, IAlertsDalRemoveAccountSubscrResult
    {
    }

    public class AlertDalGetAccountSubscriptionsParams : IAlertsDalGetAccountSubscriptionsParams
    {
        public string AccountKey { get; set; }
    }

    public class AlertsDalGetAccountSubscriptionsResult : ResultBase, IAlertsDalGetAccountSubscriptionsResult
    {
        public AlertsDalGetAccountSubscriptionsResult()
        {
            Subscriptions = new List<Subscription>();
        }

        public IList<Subscription> Subscriptions
        {
            get;
            set;
        }
    }
}
