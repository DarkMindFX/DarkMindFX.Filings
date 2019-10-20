using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using DMFX.Interfaces;
using System.Net;
using System.Threading;
using DMFX.Service.Common;

namespace DMFX.Service.Sourcing
{
    public class Global : GlobalBase
    {
        static Importer _importer = null;
        
        public static Importer Importer
        {
            get
            {
                return _importer;
            }
        }
       
        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            // initializing importer
            _importer = new Importer(Global.Container);

            new AppHost().Init();

            StartKeepAlive();

        }

        protected void Application_End(object sender, EventArgs e)
        {
            StopKeepAlive();
        }

    }
}