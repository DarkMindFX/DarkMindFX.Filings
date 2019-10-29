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
            DMFX.Client.Accounts.ServiceClient client = new DMFX.Client.Accounts.ServiceClient();

            //JsonServiceClient client = new JsonServiceClient("http://darkmindfx.com/api/accounts");
            

            GetSessionInfo request = new GetSessionInfo();
            request.SessionToken = "c664db5f-77e3-4066-97f0-4608f413c570";
            request.CheckActive = true;

            var response = client.PostGetSessionInfo(request);

           
        }
    }
}
