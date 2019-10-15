using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/SetJobActiveState/{JobCode}/{IsActive}/{SessionToken}", "GET")]
    [Route("/SetJobActiveState", "POST")]
    public class SetJobActiveState : RequestBase, IReturn<SetJobActiveStateResponse>
    {
        public string JobCode
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }
    }

    public class SetJobActiveStateResponse : ResponseBase
    {
    }
}
