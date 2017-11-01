using DMFX.Interfaces;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMFX.Service.Common
{
    public class GlobalBase : System.Web.HttpApplication
    {
        static CompositionContainer _container = null;
        static Thread keepAliveThread = new Thread(KeepAlive);
        static ILogger _logger = null;

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static CompositionContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public void InitApp(TypeCatalog externalTypeCatalog = null)
        {
            string rootFolder = Server.MapPath("~");

            JsConfig<DateTime>.SerializeFn = t => t.ToString(DateTimeFormat);

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
            if (externalTypeCatalog != null)
            {
                catalog.Catalogs.Add(externalTypeCatalog);
            }
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

            _logger = logger.Value;
        }

        public void StartKeepAlive()
        {
            if (Boolean.Parse(ConfigurationManager.AppSettings["KeepAlive"]))
            {
                keepAliveThread.Start();
                _logger.Log(EErrorType.Info, "Starting KeepAlive thread");
            }
        }

        public void StopKeepAlive()
        {
            try
            {
                _logger.Log(EErrorType.Info, "Stopping KeepAlive thread");

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
                catch (Exception)
                {
                }
            }
        }
    }
}
