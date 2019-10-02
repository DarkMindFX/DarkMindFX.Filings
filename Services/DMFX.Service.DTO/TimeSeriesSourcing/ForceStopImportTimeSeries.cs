using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceStopImportTimeSeries/{SessionToken}", "GET")]
    [Route("/ForceStopImportTimeSeries", "PUT")]
    public class ForceStopImportTimeSeries : RequestBase, IReturn<ForceStopImportTimeSeriesResponse>
    {
    }

    public class ForceStopImportTimeSeriesResponse : ResponseBase
    {
    }
}
