using DMFX.Service.Filings;
using DMFX.Service.Common;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Threading;

namespace DMFX.Service.Filings
{
    public class Global : GlobalBase
    {
        public static Semaphore SimAccounts
        {
            get;
            set;
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            SimAccounts = new Semaphore(0, 2);

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_Stop(object sender, EventArgs e)
        {
            StopKeepAlive();
        }
    }
}