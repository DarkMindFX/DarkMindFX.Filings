using DMFX.Interfaces;
using DMFX.Service.Common;
using System;

namespace DMFX.Service.Scheduler
{
    public class Global : GlobalBase
    {
        private static Scheduler _scheduler = null;

        public static Scheduler Scheduler
        {
            get
            {
                return _scheduler;
            }

        }

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            _scheduler = new Scheduler();

            Logger.Log(EErrorType.Info, "Starting service DMFX.Service.Scheduler");

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            StopKeepAlive();
        }
    }
}