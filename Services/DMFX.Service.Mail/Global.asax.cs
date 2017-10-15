﻿using DMFX.Interfaces;
using DMFX.Service.Mail.MessageGenerators;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;

namespace DMFX.Service.Mail
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
            AddMessageGenerators(catalog);
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

            logger.Value.Log(EErrorType.Info, "Starting service DMFX.Service.Mail");

            new AppHost().Init();
        }

        private void AddMessageGenerators(AggregateCatalog catalog)
        {
            var msgCatalog = new TypeCatalog(
                typeof(MsgGenAccountCreatedConfirmation)
                );

            catalog.Catalogs.Add(msgCatalog);

        }
    }
}