﻿using DMFX.Service.Common;
using System;

namespace DMFX.Service.TimeSeries
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