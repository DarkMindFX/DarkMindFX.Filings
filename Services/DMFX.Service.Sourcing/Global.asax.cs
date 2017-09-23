using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using DMFX.Interfaces;
using System.Net;
using System.Threading;

namespace DMFX.Service.Sourcing
{
    public class Global : System.Web.HttpApplication
    {
        static CompositionContainer _container = null;
        static Importer _importer = null;
        static Thread keepAliveThread = new Thread(KeepAlive);
        
        public static Importer Importer
        {
            get
            {
                return _importer;
            }
        }


        public static CompositionContainer Container
        {
            get
            {
                return _container;
            }
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            string rootFolder = Server.MapPath("~");

            // Preparing logs folder
            string logsFolder = Path.Combine(rootFolder, ConfigurationManager.AppSettings["LogsFolder"]);
            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }

            // preparing plugins folder
            string pluginsFolder = Path.Combine(rootFolder, ConfigurationManager.AppSettings["PluginsFolder"]);

            DirectoryCatalog dcatalog = new DirectoryCatalog(pluginsFolder, "*.dll");

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(dcatalog);
            _container = new CompositionContainer(catalog);
            _container.ComposeExportedValue("ServiceRootFolder", rootFolder);
            _container.ComposeParts(this);

            // initializing logger
            Lazy<ILogger> logger = _container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]);
            ILoggerParams loggerParams = logger.Value.CreateParams();

            if (ConfigurationManager.AppSettings["LoggerType"] == "FileLogger")
            {
                loggerParams.Parameters["LogFolder"] = logsFolder;
                loggerParams.Parameters["NameTemplate"] = ConfigurationManager.AppSettings["LogFileNameTemplate"];
                logger.Value.Init(loggerParams);
            }

            logger.Value.Log(EErrorType.Info, "Starting service DMFX.Service.Sourcing");

            // initializing importer
            _importer = new Importer(_container);

            new AppHost().Init();

            if (Boolean.Parse(ConfigurationManager.AppSettings["KeepAlive"]))
            {
                keepAliveThread.Start();
                logger.Value.Log(EErrorType.Info, "Starting KeepAlive thread");
            }

        }

        protected void Application_End(object sender, EventArgs e)
        {
            try
            {
                Lazy<ILogger> logger = _container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]);
                logger.Value.Dispose();

                if (Boolean.Parse(ConfigurationManager.AppSettings["KeepAlive"]))
                {
                    keepAliveThread.Abort();
                }
            }
            catch
            {
            }
        }

        static void KeepAlive()
        {
            string keepAliveUrl = string.Format(ConfigurationManager.AppSettings["KeepAliveURL"], ConfigurationManager.AppSettings["ServiceSessionToken"]);
            int keepAliveInterval = Int32.Parse(ConfigurationManager.AppSettings["KeepAliveInterval"]) * 1000;

            while (true)
            {
                try
                {
                    Thread.Sleep(keepAliveInterval);

                    WebRequest req = WebRequest.Create(keepAliveUrl);
                    req.GetResponse();

                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch(Exception ex)
                {
                }
            }
        }
    }
}