using DMFX.MQInterfaces;
using DMFX.Service.Common;
using System;
using System.Configuration;

namespace DMFX.Service.TimeSeries
{
    public class Global : GlobalBase
    {
        public static MQClient.Client MQClient
        {
            get;
            set;
        }

        public static string AccountsChannel
        {
            get;
            set;
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            InitApp();

            InitMQClient();

            new AppHost().Init();

            StartKeepAlive();
        }

        protected void Application_Stop(object sender, EventArgs e)
        {
            DeinitMQClient();

            StopKeepAlive();
        }

        private void InitMQClient()
        {
            AccountsChannel = ConfigurationManager.AppSettings["MQAccountsChannelName"];
            IMessageQueue mq = Global.Container.GetExport<IMessageQueue>(ConfigurationManager.AppSettings["MessageQueueType"]).Value;
            if (mq != null)
            {
                IMQInitParams initParams = mq.CreateInitParams();
                initParams.Params["ConnectionString"] = ConfigurationManager.AppSettings["ConnectionStringMsgBus"];

                mq.Init(initParams);

                MQClient = new MQClient.Client(mq);
                if (MQClient.Init(ConfigurationManager.AppSettings["MQSubscriberName"]))
                {
                    MQClient.Subscribe(AccountsChannel);
                }
            }
        }

        private void DeinitMQClient()
        {
            if (MQClient != null)
            {
                MQClient.Unsubscribe(AccountsChannel);
                MQClient.Dispose();
                MQClient = null;
            }
        }
    }
}