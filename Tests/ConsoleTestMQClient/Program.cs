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

            
            _clientProcessor = new Client(mq);
            _clientProcessor.Init(ConfigurationManager.AppSettings["ProcessorName"]);
            _clientProcessor.Subscribe(ConfigurationManager.AppSettings["ChannelName"]);
            _clientProcessor.NewChannelMessages += NewMessagesHandlerProcessor;

            _clientSender = new Client(mq);
            _clientSender.Init(ConfigurationManager.AppSettings["SenderName"]);
            _clientSender.Subscribe(ConfigurationManager.AppSettings["ChannelName"]);
            _clientSender.NewChannelMessages += NewMessagesHandlerSender;
            
            Task sender = new Task(ThreadSender);
            sender.Start();
            
            sender.Wait();
            

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

        private void NewMessagesHandlerSender(object sender, NewChannelMessagesDelegateEventArgs args)
        {
            foreach (var m in args.Messages)
            {
                _clientProcessor.SetMessageStatus(m.Id, EMessageStatus.Processing);

                Console.WriteLine(string.Format("[{0}] {1}: Sender got message: {2}", 
                                    DateTime.Now, 
                                    Thread.CurrentThread.ManagedThreadId, 
                                    m.Payload));

                _clientProcessor.SetMessageStatus(m.Id, EMessageStatus.Completed);
            }
        }


    }
}
