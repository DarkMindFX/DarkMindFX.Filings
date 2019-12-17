using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DMFX.Service.Echo2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public WebApiApplication()
        {

        }
        protected void Application_Start()
        {
            
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // ServicePointManager setup
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.EnableDnsRoundRobin = true;
            ServicePointManager.ReusePort = true;
        }
    }
}
