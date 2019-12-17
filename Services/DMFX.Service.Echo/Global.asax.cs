using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DMFX.Service.Echo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
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
