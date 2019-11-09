using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/Sanitize/{SessionToken}", "GET")]
    [Route("/Sanitize", "POST")]
    public class Sanitize : RequestBase, IReturn<SanitizeResponse>
    {
    }

    public class SanitizeResponse : ResponseBase
    {
    }
}
