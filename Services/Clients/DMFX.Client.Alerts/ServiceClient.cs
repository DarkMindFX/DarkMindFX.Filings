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
            GetAccountAlertsResponse response = _client.Post<GetAccountAlertsResponse>(request);

            return response;
        }

        public GetAlertsTypesResponse PostGetAlertsTypes(GetAlertsTypes request)
        {
            GetAlertsTypesResponse response = _client.Post<GetAlertsTypesResponse>(request);

            return response;
        }

        public GetNotificationTypesResponse PostGetNotificationTypes(GetNotificationTypes request)
        {
            GetNotificationTypesResponse response = _client.Post<GetNotificationTypesResponse>(request);

            return response;
        }

        public AddAccountAlertsResponse PostAddAccountAlerts(AddAccountAlerts request)
        {
            AddAccountAlertsResponse response = _client.Post<AddAccountAlertsResponse>(request);

            return response;
        }

        public UpdateAccountAlertsResponse PostUpdateAccountAlerts(UpdateAccountAlerts request)
        {
            UpdateAccountAlertsResponse response = _client.Post<UpdateAccountAlertsResponse>(request);

            return response;
        }

        public RemoveAccountAlertsResponse PostRemoveAccountAlerts(RemoveAccountAlerts request)
        {
            RemoveAccountAlertsResponse response = _client.Post<RemoveAccountAlertsResponse>(request);

            return response;
        }


    }
}
