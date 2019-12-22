using DMFX.MQClient;
using DMFX.MQDAL;
using DMFX.MQInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTestMQClient
{
    class TestSender
    {
    }
    class Program
    {
        static void Main(string[] args)
        {
            Program prg = new Program();
            prg.Start();
        }

        Client _clientSender = null;
        List<Task> _tasks = new List<Task>();
       
        public void Start()
        {

            for(int i = 0; i < 2; ++i)
            {

            }

        }

        public void SenderThread()
        {
            IMessageQueue mq = CreateMQ();

            string requestId = Guid.NewGuid().ToString();

            AutoResetEvent _eventHasMessages = null;
            string response = null;

            void NewMessagesHandlerSender(object sender, NewChannelMessagesDelegateEventArgs args)
            {
                foreach (var m in args.Messages)
                {                 
                    response = m.Payload;
                    _clientSender.DeleteMessage(m.Id);

                    _eventHasMessages.Set();
                }
            }

            
            _eventHasMessages = new AutoResetEvent(false);
            _clientSender = new Client(mq);
            _clientSender.Init(ConfigurationManager.AppSettings["SenderName"]);
            _clientSender.Subscribe("AccountRequestsChannel-DEV");
            _clientSender.NewChannelMessages += NewMessagesHandlerSender;

            
            // getting session info
            string payload = "{ \"SessionToken\":\"e5e2685f-bce5-47ca-b4d7-85a0a940a7d7\", " +
                "               \"RequestID\":\""+ requestId +"\"," +
                "               \"CheckActive\": true }";

            _clientSender.Push("AccountRequestsChannel-DEV", "GetSessionInfo", payload);
            _eventHasMessages.WaitOne();
            _clientSender.NewChannelMessages -= NewMessagesHandlerSender;
            

            Console.WriteLine(string.Format("[{0}] {1}: Sender got message:\r\n{2}",
                                        DateTime.Now,
                                        Thread.CurrentThread.ManagedThreadId,
                                        response));

            Thread.Sleep(100000);

        }

        private IMessageQueue CreateMQ()
        {
            var mq = new DbMessageQueue();
            IMQInitParams paramsInit = mq.CreateInitParams();
            paramsInit.Params["ConnectionString"] = ConfigurationManager.AppSettings["ConnectionStringMsgBus"];
            mq.Init(paramsInit);

            return mq;
        }


        private void ThreadSender()
        {
            Console.WriteLine("Sender thread started");
            Random rnd = new Random(DateTime.Now.Millisecond);
            for(int i = 0; i < 3; ++i)
            {
                _clientSender.Push(ConfigurationManager.AppSettings["ChannelName"], "TestType", string.Format("TestPayload-{0}", i));

                Thread.Sleep(rnd.Next(2000, 3000));
            }

            Console.WriteLine("Sender thread complete");
        }

        


    }
}
