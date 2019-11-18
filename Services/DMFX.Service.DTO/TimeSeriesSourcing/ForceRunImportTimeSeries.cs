using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceRunImportTimeSeries/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DaysBack}/{AgencyCode}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DaysBack}/{AgencyCode}/{SymbolCodes}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DateStart}/{DateEnd}/{AgencyCode}/{SymbolCodes}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries", "POST")]
    public class ForceRunImportTimeSeries : RequestBase, IReturn<ForceRunImportTimeSeriesResponse>
    {
        public DateTime? DateStart
        {
            get;
            set;
        }

        public DateTime? DateEnd
        {
            get;
            set;
        }

        public string[] SymbolCodes
        {
            get;
            set;
        }

        public decimal? DaysBack
        {
            get;
            set;
        }

        public ETimeFrame? Timeframe
        {
            get;
            set;
        }

        public string AgencyCode
        {
            get;
            set;
        }
    }

    public class ForceRunImportTimeSeriesResponse : ResponseBase
    {
    }
}
