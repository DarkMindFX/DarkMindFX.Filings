using DMFX.Service.Accounts;
using DMFX.Service.Common;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DMFX.Service.Accounts
{
    public class Global : GlobalBase
    {
        public static AccountRequestsProcessor RequestsProcessor
        {
            get;
            set;
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            RequestsProcessor = new AccountRequestsProcessor();

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_Stop(object sender, EventArgs e)
        {
            if(RequestsProcessor != null)
            {
                RequestsProcessor.Dispose();
                RequestsProcessor = null;
            }
            StopKeepAlive();
        }
    }
}