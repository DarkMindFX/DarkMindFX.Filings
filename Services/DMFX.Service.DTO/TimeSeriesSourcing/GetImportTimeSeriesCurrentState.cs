using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO.TimeSeriesSourcing
{
    [Route("/GetImporterTimeSeriesState/{SessionToken}", "GET")]
    [Route("/GetImporterTimeSeriesState", "POST")]
    public class GetImporterTimeSeriesState : RequestBase, IReturn<GetImporterTimeSeriesStateResponse>
    {
    }

    public class GetImporterTimeSeriesStateResponse : ResponseBase
    {
        public GetImporterTimeSeriesStateResponse()
        {
            TimeSeriesProcessed = new List<TickerInfo>();
        }
        public string State
        {
            get;
            set;
        }

        public DateTime? LastImportRun
        {
            get;
            set;
        }

        public DateTime? LastImportEnd
        {
            get;
            set;
        }

        public int ProcessedCount
        {
            get;
            set;
        }

        public List<TickerInfo> TimeSeriesProcessed
        {
            get;
            set;
        }
    }
}
