using DMFX.QuotesInterfaces;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO.TimeSeries
{
    [Route("/GetTimeSeries/{CountryCode}/{Ticker}/{SessionToken}", "GET")]
    [Route("/GetTimeSeries", "POST")]
    public class GetTimeSeriesInfo : RequestBase, IReturn<GetTimeSeriesInfoResponse>
    {
        public string CountryCode
        {
            get;
            set;
        }

        public string Ticker
        {
            get;
            set;
        }

        public EUnit Unit
        {
            get;
            set;
        } 
    }

    public class TimeSeriesInfoItem
    {
        public ETimeFrame TimeFrame
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public DateTime? LastUpdated
        {
            get;
            set;
        }
    }

    public class GetTimeSeriesInfoResponse : ResponseBase
    {
        public string CountryCode
        {
            get;
            set;
        }

        public string Ticker
        {
            get;
            set;
        }

        public EUnit Unit
        {
            get;
            set;
        }

        public ETimeSeriesType Type
        {
            get;
            set;
        }

        public IList<TimeSeriesInfoItem> Series
        {
            get;
            set;
        }
    }
}
