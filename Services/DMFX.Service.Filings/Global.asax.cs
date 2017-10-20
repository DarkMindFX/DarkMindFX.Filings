using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using DMFX.Interfaces;
using DMFX.Service.Common;

namespace DMFX.Service.Filings
{
    public class Global : GlobalBase
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            Logger.Log(EErrorType.Info, "Starting service DMFX.Service.Filings");

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            StopKeepAlive();
        }
    }
}