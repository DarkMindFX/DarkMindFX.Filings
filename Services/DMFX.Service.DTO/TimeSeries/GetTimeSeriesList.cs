using DMFX.QuotesInterfaces;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO.TimeSeries
{
    [Route("/GetTimeSeries/{CountryCode}/{Type}/{SessionToken}", "GET")]
    [Route("/GetTimeSeries", "POST")]
    public class GetTimeSeriesList : RequestBase , IReturn<GetTimeSeriesListResponse>
    {
        public string CountryCode
        {
            get;
            set;
        }

        public ETimeSeriesType Type
        {
            get;
            set;
        }
    }

    public class TimeSeriesListItem
    {
        public string Ticker
        {
            get;
            set;
        }

        public string CountryCode
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

    }

    public class GetTimeSeriesListResponse : ResponseBase
    {
        public string CountryCode
        {
            get;
            set;
        }

        public IList<TimeSeriesListItem> TimeSeries
        {
            get;
            set;
        }
    }
}
