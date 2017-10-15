using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.InfraStartup
{
    class InfraStartup
    {
        CompositionContainer _container = null;

        public void Start()
        {
            try
            {

                // Preparing logs folder
                string rootFolder = AssemblyDirectory;
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
                _container.ComposeParts();

                // initializing logger
                Lazy<ILogger> logger = _container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]);
                ILoggerParams loggerParams = logger.Value.CreateParams();

                if (ConfigurationManager.AppSettings["LoggerType"] == "FileLogger")
                {
                    loggerParams.Parameters["LogFolder"] = logsFolder;
                    loggerParams.Parameters["NameTemplate"] = ConfigurationManager.AppSettings["LogFileNameTemplate"];
                    logger.Value.Init(loggerParams);
                }

                logger.Value.Log(EErrorType.Info, "InfraStartup initialized");

                logger.Value.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error occured: {0}", ex.Message));
            }
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
    class Program
    {
        static InfraStartup _infraStartup = null;
        static void Main(string[] args)
        {
            _infraStartup = new InfraStartup();
            _infraStartup.Start();
        }

        
    }
}
