using DMFX.Service.DTO;
using Newtonsoft.Json;
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

        protected WebClient _client = null;
        protected string _serviceUrl = null;

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
            _serviceUrl = initParams.ServiceUrl;
            _client = new WebClient();
            _client.Headers["content-type"] = "application/json";
        }

        public EchoResponse PostEcho(Echo request)
        {
            EchoResponse response = Post<Echo, EchoResponse>( "Echo", request);
            return response;
        }

        protected TResponse Post<TRequest,TResponse>(string method, TRequest request)
        {
            byte[] reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(request, Formatting.Indented));
            byte[] resByte = _client.UploadData(_serviceUrl + "json/reply/" + method, "post", reqString);
            string resString = Encoding.Default.GetString(resByte);

            TResponse response = JsonConvert.DeserializeObject<TResponse>(resString);

            //TResponse response = _client.Post<TResponse>("/json/reply/" + method, request);

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
