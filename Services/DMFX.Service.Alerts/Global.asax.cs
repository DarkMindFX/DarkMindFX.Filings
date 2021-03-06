﻿using DMFX.Service.Alerts;
using DMFX.Service.Common;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DMFX.Service.Alerts
{
    public class Global : GlobalBase
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_Stop(object sender, EventArgs e)
        {
            StopKeepAlive();
        }
    }
}