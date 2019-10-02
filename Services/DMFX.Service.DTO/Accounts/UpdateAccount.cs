using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/UpdateAccount/{Email}/{Name}/{Pwd}/{SessionToken}", "GET")]
    [Route("/UpdateAccount", "POST")]
    public class UpdateAccount : RequestBase, IReturn<UpdateAccountResponse>
    {
        public string Email
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Pwd
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }
    }

    public class UpdateAccountResponse : ResponseBase
    {
    }
}
