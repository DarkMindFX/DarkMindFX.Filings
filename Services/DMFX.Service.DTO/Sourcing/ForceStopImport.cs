using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceStopImport/{SessionToken}", "GET")]
    [Route("/ForceStopImport", "POST")]
    public class ForceStopImport : RequestBase, IReturn<ForceStopImportResponse>
    {     
    }

    public class ForceStopImportResponse : ResponseBase
    {
    }
}
