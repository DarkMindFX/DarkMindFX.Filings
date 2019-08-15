using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetTimeSeries/{CountryCode}/{Ticker}/{TimeFrame}/{SessionToken}", "GET")]
    [Route("/GetTimeSeries/{CountryCode}/{Ticker}/{TimeFrame}/{PeriodStart}/{PeriodEnd}/{SessionToken}", "GET")]
    [Route("/GetTimeSeries", "POST")]
    public class GetTimeSeries : RequestBase, IReturn<GetQuotesResponse>
    {
        public string Ticker
        {
            get;
            set;
        }

        public DateTime? PeriodStart
        {
            get;
            set;
        }

        public DateTime? PeriodEnd
        {
            get;
            set;
        }

        public ETimeFrame TimeFrame
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }
    }

    public class GetQuotesResponse : ResponseBase
    {
        public TickerQuotes Quotes
        {
            get;
            set;
        }
    }
}
