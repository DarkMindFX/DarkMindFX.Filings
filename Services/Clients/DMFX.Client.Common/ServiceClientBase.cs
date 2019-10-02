using ServiceStack;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Client.Common
{
    public class ServiceClientBaseInitParams
    {
        public string ServiceUrl
        {
            get;
            set;
        }
    }

    public abstract class ServiceClientBase : IDisposable
    {       

        protected JsonServiceClient _client = null;

        public ServiceClientBase()
        {
        }

        public void Init(ServiceClientBaseInitParams initParams)
        {
            _client = new JsonServiceClient(initParams.ServiceUrl);
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
