using DMFX.Interfaces;
using DMFX.Service.Common;
using DMFX.Service.Mail.MessageGenerators;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.IO;

namespace DMFX.Service.Mail
{
    public class Global : GlobalBase
    {
        

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp(PrepareMessageGenerators());

            PrepareMessageGenerators();

            Logger.Log(EErrorType.Info, "Starting service DMFX.Service.Mail");

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            StopKeepAlive();
        }

        private TypeCatalog PrepareMessageGenerators()
        {
            var msgCatalog = new TypeCatalog(
                typeof(MsgGenAccountCreatedConfirmation)
                );

            return msgCatalog;

        }
    }
}