using DMFX.Client.TechUtils.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Client.TechUtils
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

        public SanitizeResponse PostSanitize(Sanitize request)
        {
            SanitizeResponse response = Post<Sanitize, SanitizeResponse>("Sanitize", request);

            return response;
        }
    }
}
