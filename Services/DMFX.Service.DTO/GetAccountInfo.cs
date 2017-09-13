using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/CreateAccount/{Email}/{Name}/{Pwd}", "GET")]
    [Route("/CreateAccount", "POST")]
    public class GetAccountInfo : RequestBase, IReturn<GetAccountInfoResponse>
    {
        public string AccountKey
        {
            get;
            set;
        }
    }

    public class GetAccountInfoResponse : ResponseBase
    {
    }
}
