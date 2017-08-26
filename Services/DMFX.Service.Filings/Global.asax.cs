using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;

namespace DMFX.Service.Filings
{
    public class Global : System.Web.HttpApplication
    {
        static CompositionContainer _container = null;

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
            string logsFolder = Path.Combine(rootFolder, ConfigurationManager.AppSettings["LogsFolder"]);
            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }
            string logFilePath = Path.Combine(logsFolder, "DMFX.Service.Filings_" + DateTime.Now.ToString().Replace("/", "-").Replace(":", "-") + ".log");
            string pluginsFolder = Path.Combine(rootFolder, ConfigurationManager.AppSettings["PluginsFolder"]);

            DirectoryCatalog dcatalog = new DirectoryCatalog(pluginsFolder, "*.dll");

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(dcatalog);
            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            new AppHost().Init();
        }
    }
}