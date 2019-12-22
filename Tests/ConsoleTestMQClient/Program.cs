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
        Client _clientProcessor = null;
        

        public void Start()
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
                    _clientSender.SetMessageStatus(m.Id, EMessageStatus.Completed);

                    _eventHasMessages.Set();
                }
            }

            /*
            _clientProcessor = new Client(mq);
            _clientProcessor.Init(ConfigurationManager.AppSettings["ProcessorName"]);
            _clientProcessor.Subscribe(ConfigurationManager.AppSettings["ChannelName"]);
            _clientProcessor.NewChannelMessages += NewMessagesHandlerProcessor;
            */
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
            /*
            Task sender = new Task(ThreadSender);
            sender.Start();
            
            sender.Wait();
            */

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

        private void NewMessagesHandlerProcessor(object sender, NewChannelMessagesDelegateEventArgs args)
        {
            foreach(var m in args.Messages)
            {
                _clientProcessor.SetMessageStatus(m.Id, EMessageStatus.Processing);

                string respMsg = m.Payload + " -> Processed";

                Console.WriteLine(string.Format("[{0}] {1}: Processor got message: {2}, Responding: {3}",
                                    DateTime.Now,
                                    Thread.CurrentThread.ManagedThreadId,
                                    m.Payload,
                                    respMsg));

                _clientProcessor.SetMessageStatus(m.Id, EMessageStatus.Completed);

                _clientProcessor.Push(ConfigurationManager.AppSettings["ChannelName"], "TestType", respMsg, null);
            }
        } 

        /*
        private void NewMessagesHandlerSender(object sender, NewChannelMessagesDelegateEventArgs args)
        {
            foreach (var m in args.Messages)
            {
                Console.WriteLine(string.Format("[{0}] {1}: Sender got message: {2}", 
                                    DateTime.Now, 
                                    Thread.CurrentThread.ManagedThreadId, 
                                    m.Payload));

                _clientSender.SetMessageStatus(m.Id, EMessageStatus.Completed);
            }
        }
        */


    }
}
