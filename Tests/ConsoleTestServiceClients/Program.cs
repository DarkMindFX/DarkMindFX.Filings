using DMFX.Service.DTO;
using Newtonsoft.Json;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestServiceClients
{
    class WebClientEx : WebClient
    {
        public WebClientEx()
        {
            Timeout = 120000;
        }

        public int Timeout
        {
            get;
            set;
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
            req.Timeout = this.Timeout;
            req.ServicePoint.ConnectionLimit = 10;
            return (WebRequest)req;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // ServicePointManager setup
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.EnableDnsRoundRobin = true;
            ServicePointManager.ReusePort = true;
            int tasksCount = 3;
            
            List<Task> importTasks = new List<Task>();

            for(int i = 0; i < tasksCount; ++i)
            {
                Task importTask = new Task(() => ThreadEcho());
                importTasks.Add(importTask);
                importTask.Start();
            }

            Task.WaitAll(importTasks.ToArray());

        }

        private static void ThreadEcho()
        {
            int count = 10;           

            var rnd = new Random(DateTime.Now.Millisecond);

            while (count > 0)
            {
                try
                {
                    string message = Guid.NewGuid().ToString();
                    string echoResp = string.Empty;

                    using (var client = new HttpClient())
                    {

                        var sResp = client.GetAsync("http://localhost/api/echoservice/Echo/" + message + "/2/3").Result;

                        if (sResp.StatusCode == HttpStatusCode.OK)
                        {
                            var stringResult = sResp.Content.ReadAsStringAsync().Result;
                            EchoResponse echoResponse = JsonConvert.DeserializeObject<EchoResponse>(stringResult);

                            echoResp = echoResponse.Payload.Message;
                        }
                        
                    }

                    Console.WriteLine(string.Format("[{0}] [{1}] Request completed: {2} ",
                        DateTime.Now,
                        System.Threading.Thread.CurrentThread.ManagedThreadId,
                        echoResp));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format("[{0}] [{1}] Error: {2} ",
                        DateTime.Now,
                        System.Threading.Thread.CurrentThread.ManagedThreadId,
                        ex.Message));
                }

                --count;

                System.Threading.Thread.Sleep(rnd.Next(100, 500));
            }

        }
    }
}
