using DMFX.Client.Alerts.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Client.Alerts
{
    public class ServiceClient : ServiceClientBase
    {
        string serviceUrl = null;

        public ServiceClient()
        {
            serviceUrl = Resources.ServiceUrl;
            ServiceClientBaseInitParams initParams = new ServiceClientBaseInitParams();
            initParams.ServiceUrl = serviceUrl;

            Init(initParams);
        }

        public GetAccountAlertsResponse PostGetAccountAlerts(GetAccountAlerts request)
        {
            GetAccountAlertsResponse response = Post<GetAccountAlerts, GetAccountAlertsResponse>("GetAccountAlerts", request);

            return response;
        }

        public GetAlertsTypesResponse PostGetAlertsTypes(GetAlertsTypes request)
        {
            GetAlertsTypesResponse response = Post<GetAlertsTypes, GetAlertsTypesResponse>("GetAccountAlerts", request);

            return response;
        }

        public GetNotificationTypesResponse PostGetNotificationTypes(GetNotificationTypes request)
        {
            GetNotificationTypesResponse response = Post<GetNotificationTypes, GetNotificationTypesResponse>("GetNotificationTypes", request);

            return response;
        }

        public AddAccountAlertsResponse PostAddAccountAlerts(AddAccountAlerts request)
        {
            AddAccountAlertsResponse response = Post<AddAccountAlerts, AddAccountAlertsResponse>("AddAccountAlerts", request);

            return response;
        }

        public UpdateAccountAlertsResponse PostUpdateAccountAlerts(UpdateAccountAlerts request)
        {
            UpdateAccountAlertsResponse response = Post<UpdateAccountAlerts, UpdateAccountAlertsResponse>("UpdateAccountAlerts", request);

            return response;
        }

        public RemoveAccountAlertsResponse PostRemoveAccountAlerts(RemoveAccountAlerts request)
        {
            RemoveAccountAlertsResponse response = Post<RemoveAccountAlerts, RemoveAccountAlertsResponse>("RemoveAccountAlerts", request);

            return response;
        }


    }
}
