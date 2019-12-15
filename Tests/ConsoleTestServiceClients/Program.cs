using DMFX.Service.DTO;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestServiceClients
{
    class Program
    {
        static void Main(string[] args)
        {
            int tasksCount = 3;

            List<Task> importTasks = new List<Task>();

            for(int i = 0; i < tasksCount; ++i)
            {
                Task importTask = new Task(() => ThreadFilings());
                importTasks.Add(importTask);
                importTask.Start();
            }

            Task.WaitAll(importTasks.ToArray());

        }

        private static void ThreadAccount()
        {
            int count = 100;

            var rnd = new Random(DateTime.Now.Millisecond);

            while (count > 0)
            {
                

                DMFX.Client.Accounts.ServiceClient client = new DMFX.Client.Accounts.ServiceClient();

                GetSessionInfo request = new GetSessionInfo();
                request.SessionToken = "c664db5f-77e3-4066-97f0-4608f413c570";
                request.CheckActive = true;

                var response = client.PostGetSessionInfo(request);

                Console.WriteLine(string.Format("[{0}] [{1}] Request completed: {2} ", 
                    DateTime.Now,
                    System.Threading.Thread.CurrentThread.ManagedThreadId, 
                    response.RequestID));

                --count;

                System.Threading.Thread.Sleep(rnd.Next(100, 500));
            }

        }

        private static void ThreadFilings()
        {
            int count = 100;

            var rnd = new Random(DateTime.Now.Millisecond);

            while (count > 0)
            {


                DMFX.Client.Filings.ServiceClient client = new DMFX.Client.Filings.ServiceClient();

                GetRegulators request = new GetRegulators();
                request.SessionToken = Guid.NewGuid().ToString();
 
                var response = client.PostGetRegulators(request);

                Console.WriteLine(string.Format("[{0}] [{1}] Request completed: {2} ",
                    DateTime.Now,
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    response.RequestID));

                --count;

                System.Threading.Thread.Sleep(rnd.Next(100, 500));
            }

        }
    }
}
