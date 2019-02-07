using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceStopImportQuotes/{SessionToken}", "GET")]
    [Route("/ForceStopImportQuotes", "PUT")]
    public class ForceStopImportQuotes : RequestBase, IReturn<ForceStopImportQuotesResponse>
    {
    }

    public class ForceStopImportQuotesResponse : ResponseBase
    {
    }
}
