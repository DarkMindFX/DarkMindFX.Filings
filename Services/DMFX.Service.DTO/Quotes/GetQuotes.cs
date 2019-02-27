using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetCompanies/{CountryCode}/{Ticker}/{TimeFrame}/{SessionToken}", "GET")]
    [Route("/GetCompanies/{CountryCode}/{Ticker}/{TimeFrame}/{PeriodStart}/{PeriodEnd}/{SessionToken}", "GET")]
    [Route("/GetCompanies", "POST")]
    public class GetQuotes : RequestBase, IReturn<GetQuotesResponse>
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
