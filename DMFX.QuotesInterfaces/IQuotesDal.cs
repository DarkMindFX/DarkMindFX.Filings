using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesInterfaces
{
    public interface IQuotesDalInitParams
    {
        Dictionary<string, string> Parameters
        {
            get;
        }
    }

    public interface IQuotesDalGetTimeSeriesValuesParams
    {
        List<string> Tickers
        {
            get;
            set;
        }

        string Country
        {
            get;
            set;
        }

        DateTime PeriodStart
        {
            get;
            set;
        }

        DateTime PeriodEnd
        {
            get;
            set;
        }

        ETimeFrame TimeFrame
        {
            get;
            set;
        }
    }

    public interface IQuotesDalGetTimeseriesValuesResult : IResult
    {
        List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    public interface IQuotesDalSaveTimeseriesValuesParams
    {

        List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    public interface IQuotesDalSaveTimeseriesValuesResult : IResult
    {       
        uint TimeSeriesSaved
        {
            get;
            set;
        } 
    }

    public interface IQuotesDalGetTickersListParams
    {
        string CountryCode
        {
            get;
            set;
        }

        ETimeSeriesType Type
        {
            get;
            set;
        }
    }

    public class TickersListItem
    {
        public string Ticker
        {
            get;
            set;
        }

        public string Name
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

    public interface IQuotesDalGetTickersListResult : IResult
    {
        IList<TickersListItem> Tickers
        {
            get;
            set;
        }
    }

    public interface IQuotesDalGetTimeSeriesInfoParams
    {
        string Ticker
        {
            get;
            set;
        }

        string CountryCode
        {
            get;
            set;
        }
    }

    public class TimeSeriesInfoListItem
    {
        public ETimeFrame Timeframe
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

            
    }

    public interface IQuotesDalGetTimeSeriesInfoResult : IResult
    {
        string Ticker
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        string CountryCode
        {
            get;
            set;
        }

        IList<TimeSeriesInfoListItem> Series
        {
            get;
            set;
        }

        IList<string> Columns
        {
            get;
            set;
        }

        EUnit Unit
        {
            get;
            set;
        }

        ETimeSeriesType Type
        {
            get;
            set;
        }

        
    }

    public interface IQuotesDal
    {
        /// <summary>
        /// Method to init DAL
        /// </summary>
        /// <param name="initParams"></param>
        void Init(IQuotesDalInitParams initParams);

        /// <summary>
        /// Returns quotes for a given list of tickers
        /// </summary>
        /// <param name="getQuotesParams"></param>
        /// <returns></returns>
        IQuotesDalGetTimeseriesValuesResult GetTimseriesValues(IQuotesDalGetTimeSeriesValuesParams getQuotesParams);


        /// <summary>
        /// Returns list of time series / tickers corresponding to given params
        /// </summary>
        /// <param name="getTsList"></param>
        /// <returns></returns>
        IQuotesDalGetTickersListResult GetTickersList(IQuotesDalGetTickersListParams getTsList);

        /// <summary>
        /// Returns list of time series / tickers corresponding to given params
        /// </summary>
        /// <param name="getTsList"></param>
        /// <returns></returns>
        IQuotesDalGetTimeSeriesInfoResult GetTimeSeriesInfo(IQuotesDalGetTimeSeriesInfoParams getTsInfoParams);


        /// <summary>
        /// Saves the list of ticker values to storage
        /// </summary>
        /// <param name="saveQuotesParams"></param>
        /// <returns></returns>
        IQuotesDalSaveTimeseriesValuesResult SaveTimeseriesValues(IQuotesDalSaveTimeseriesValuesParams saveQuotesParams);

        IQuotesDalInitParams CreateInitParams();

        IQuotesDalGetTimeSeriesValuesParams CreateGetQuotesParams();

        IQuotesDalSaveTimeseriesValuesParams CreateSaveTimeseriesValuesParams();

        IQuotesDalGetTimeSeriesInfoParams CreateGetTimeSeriesInfoParams();

        IQuotesDalGetTickersListParams CreateGetTickersListParams();


    }
}
