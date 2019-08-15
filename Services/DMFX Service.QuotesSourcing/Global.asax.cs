using DMFX.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DMFX.Service.QuotesSourcing
{
    public class Global : GlobalBase
    {

        static TimeSeriesImporter _importer = null;

        public static TimeSeriesImporter Importer
        {
            get
            {
                return _importer;
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            _importer = new TimeSeriesImporter(Global.Container);

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_Stop(object sender, EventArgs e)
        {
            StopKeepAlive();
        }
    }
}