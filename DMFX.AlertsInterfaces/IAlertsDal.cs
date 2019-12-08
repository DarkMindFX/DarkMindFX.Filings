using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.AlertsInterfaces
{
    public class AlertType
    {
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

        public string Desc
        {
            get;
            set;
        }
    }

    public class AlertNotificationType
    {
        public long ID
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

    public class Subscription
    {
        public Subscription()
        {
            SubscriptionData = new Dictionary<string, string>();
        }

        public long Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string AccountKey
        {
            get;
            set;
        }

        public long TypeId
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public long? NotificationTypeId
        {
            get;
            set;
        }

        public string NotificationTypeName
        {
            get;
            set;
        }

        public DateTime? SubscribedDttm
        {
            get;
            set;
        }

        public long StatusId
        {
            get;
            set;
        }

        public string StatusName
        {
            get;
            set;
        }

        public IDictionary<string, string> SubscriptionData
        {
            get;
            set;
        }
    }

    public interface IAlertsDalInitParams
    {
        IDictionary<string, string> Parameters
        {
            get;
        }
    }

    public interface IAlertsDalGetAlertTypesParams
    {
    }

    public interface IAlertsDalGetAlertTypesResult : IResult
    {
        IList<AlertType> Types
        {
            get;
        }

    }

    public interface IAlertsDalGetAlertNotificationTypesParams
    {
    }

    public interface IAlertsDalGetAlertNotificationTypesResult : IResult
    {
        IList<AlertNotificationType> Types
        {
            get;
        }

    }

    public interface IAlertsDalAddAccountSubscrParams
    {
        Subscription SubscriptonDetails
        {
            get;
            set;
        }
    }

    public interface IAlertsDalAddAccountSubscrResult : IResult
    {
    }

    public interface IAlertsDalUpdateAccountSubscrParams
    {
        Subscription SubscriptonDetails
        {
            get;
            set;
        }
    }

    public interface IAlertsDalUpdateAccountSubscrResult : IResult
    {
    }

    public interface IAlertsDalRemoveAccountSubscrParams
    {
        IList<long> SubscriptionIds
        {
            get;
            set;
        }
    }

    public interface IAlertsDalRemoveAccountSubscrResult : IResult
    {
    }

    public interface IAlertsDalRemoveAccountSubscrAllParams
    {
        string AccountKey
        {
            get;
            set;
        }
    }

    public interface IAlertsDalRemoveAccountSubscrAllResult : IResult
    {
    }

    public interface IAlertsDalGetAccountSubscriptionsParams
    {
        string AccountKey
        {
            get;
            set;
        }

    }

    public interface IAlertsDalGetAccountSubscriptionsResult : IResult
    {
        IList<Subscription> Subscriptions
        {
            get;
        }
    }

    public interface IAlertsDalSetAlertStatusParams
    {
        IList<Subscription> Subscriptions
        {
            get;
            set;
        }

        
    }

    public interface IAlertsDalSetAlertStatusResult : IResult
    {
    }


    public interface IAlertsDal
    {

        /// <summary>
        /// Method to init DAL
        /// </summary>
        /// <param name="initParams"></param>
        void Init(IAlertsDalInitParams initParams);

        /// <summary>
        /// Returns list of available alert types
        /// </summary>
        /// <param name="getParams"></param>
        /// <returns></returns>
        IAlertsDalGetAlertTypesResult GetAlertTypes(IAlertsDalGetAlertTypesParams getParams);

        /// <summary>
        /// Returns list of available types of notification
        /// </summary>
        /// <param name="getParams"></param>
        /// <returns></returns>
        IAlertsDalGetAlertNotificationTypesResult GetAlertNotificationTypes(IAlertsDalGetAlertNotificationTypesParams getParams);

        /// <summary>
        /// Adds subscription for given account for given alert
        /// </summary>
        /// <param name="addSubscrParams"></param>
        /// <returns></returns>
        IAlertsDalAddAccountSubscrResult AddAlertSubscription(IAlertsDalAddAccountSubscrParams addSubscrParams);


        /// <summary>
        /// Updates subscription for given account for given alert
        /// </summary>
        /// <param name="updSubscrParams"></param>
        /// <returns></returns>
        IAlertsDalUpdateAccountSubscrResult UpdateAlertSubscription(IAlertsDalUpdateAccountSubscrParams updSubscrParams);


        /// <summary>
        /// Updates subscription for given account for given set of alerts
        /// </summary>
        /// <param name="remSubscrParams"></param>
        /// <returns></returns>
        IAlertsDalRemoveAccountSubscrResult RemoveAlertSubscription(IAlertsDalRemoveAccountSubscrParams remSubscrParams);

        /// <summary>
        /// Removes all subscriptions for the given accounts
        /// </summary>
        /// <param name="remSubscrParams"></param>
        /// <returns></returns>
        IAlertsDalRemoveAccountSubscrAllResult RemoveAlertSubscriptions(IAlertsDalRemoveAccountSubscrAllParams remSubscrParams);

        /// <summary>
        /// Returns list of all subscriptions for the given account
        /// </summary>
        /// <param name="getSubsParams"></param>
        /// <returns></returns>
        IAlertsDalGetAccountSubscriptionsResult GetAccountSubscriptions(IAlertsDalGetAccountSubscriptionsParams getSubsParams);

        /// <summary>
        /// Method to change alert subscription status
        /// </summary>
        /// <param name="setSubsStatusParams"></param>
        /// <returns></returns>
        IAlertsDalSetAlertStatusResult SetAlertSubscriptionStatus(IAlertsDalSetAlertStatusParams setSubsStatusParams);




        IAlertsDalInitParams CreateInitParams();

        IAlertsDalGetAlertTypesParams CreateGetAlertTypesParams();

        IAlertsDalGetAlertNotificationTypesParams CreateGetAlertNotificationTypesParams();

        IAlertsDalAddAccountSubscrParams CreateAddAccountSubscrParams();

        IAlertsDalRemoveAccountSubscrParams CreateRemoveAccountSubscrParams();

        IAlertsDalUpdateAccountSubscrParams CreateUpdateAccountSubscrParams();

        IAlertsDalGetAccountSubscriptionsParams CreateGetAccountSubscrParams();

        IAlertsDalRemoveAccountSubscrAllParams CreateRemoveAccountSubscrAllParams();

        IAlertsDalSetAlertStatusParams CreateSetAlertStatusParams();

    }
}
