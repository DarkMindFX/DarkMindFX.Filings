using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/Login/{Email}/{Pwd}", "GET")]
    [Route("/Login", "POST")]
    public class Login : RequestBase, IReturn<LoginResponse>
    {
        public string Email
        {
            get;
            set;
        }

        public string Pwd
        {
            get;
            set;
        }
    }

    public class LoginResponse : ResponseBase
    {
    }
}
