using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/CloseSession")]
    public class CloseSession : RequestBase, IReturn<CloseSessionResponse>
    {
    }

    public class CloseSessionResponse : ResponseBase
    {
    }
}
