using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetSessionInfo/{CheckActive}/{SessionToken}", "GET")]
    [Route("/GetSessionInfo", "POST")]
    public class GetSessionInfo : RequestBase, IReturn<GetSessionInfoResponse>
    {
        public GetSessionInfo()
        {
            CheckActive = false;
        }

        public bool CheckActive
        {
            get;
            set;
        }
    }

    public class GetSessionInfoResponse : ResponseBase
    {
        public DateTime SessionStart
        {
            get;
            set;
        }

        public DateTime SessionEnd
        {
            get;
            set;
        }
    }
}
