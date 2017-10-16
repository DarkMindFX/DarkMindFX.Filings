using DMFX.Interfaces;
using DMFX.Service.Common;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;

namespace DMFX.Service.Accounts
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