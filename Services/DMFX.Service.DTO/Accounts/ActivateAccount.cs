using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ActivateAccount/{Email}/{ActivationCode}", "GET")]
    [Route("/ActivateAccount", "POST")]
    public class ActivateAccount : RequestBase, IReturn<ActivateAccountResponse>
    {
        public string Email
        {
            get;
            set;
        }

        public string ActivationCode
        {
            get;
            set;
        }
    }

    public class ActivateAccountResponse : ResponseBase
    {
    }
}
