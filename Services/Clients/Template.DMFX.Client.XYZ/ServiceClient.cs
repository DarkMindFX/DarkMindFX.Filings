using Template.DMFX.Client.XYZ.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Template.DMFX.Client.XYZ
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

        public ResponseBase PostGetSessionInfo(RequestBase request)
        {
            ResponseBase response = _client.Post<ResponseBase>(request);

            return response;
        }
    }
}
