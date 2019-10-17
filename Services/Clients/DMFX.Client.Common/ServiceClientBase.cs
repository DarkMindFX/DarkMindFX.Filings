using DMFX.Service.DTO;
using ServiceStack;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
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
            // TODO: WARNING! Security issue! Remove this code when https is enabled
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );
        }

        public void Init(ServiceClientBaseInitParams initParams)
        {
            _client = new JsonServiceClient(initParams.ServiceUrl);
        }

        public EchoResponse PostEcho(Echo request)
        {
            EchoResponse response = Post<Echo, EchoResponse>( "Echo", request);
            return response;
        }

        protected TResponse Post<TRequest,TResponse>(string method, TRequest request)
        {
            TResponse response = _client.Post<TResponse>("/json/reply/" + method, request);

            return response;
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
