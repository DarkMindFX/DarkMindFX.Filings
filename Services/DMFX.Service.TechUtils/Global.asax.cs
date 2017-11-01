using DMFX.Interfaces;
using DMFX.Service.Common;
using System;

namespace DMFX.Service.TechUtils
{
    public class Global : GlobalBase
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            Logger.Log(EErrorType.Info, "Starting service DMFX.Service.TechUtils");


            new AppHost().Init();

            StartKeepAlive();

        }

        protected void Application_End(object sender, EventArgs e)
        {
            StopKeepAlive();
        }
    }
}