using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesDAL
{
    class QuotesDalMSSQLInitParams : IQuotesDalInitParams
    {
        public QuotesDalMSSQLInitParams()
        {
            Parameters = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Parameters
        {
            get;
            set;
        }
    }

    class QuotesDalMSSQLGetQuotesParams : IQuotesDalGetTimeSeriesValuesParams
    {
        public QuotesDalMSSQLGetQuotesParams()
        {
            Country = "US";
            PeriodEnd = DateTime.Now;
            PeriodStart = DateTime.Parse("1990/1/1");
            TimeFrame = ETimeFrame.Daily;

            Tickers = new List<string>();
        }

        public string Country
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public List<string> Tickers
        {
            get;
            set;
        }

        public ETimeFrame TimeFrame
        {
            get;
            set;
        }
    }

    class QuotesDalMSSQLGetQuotesResult : ResultBase, IQuotesDalGetTimeseriesValuesResult
    {
        public QuotesDalMSSQLGetQuotesResult()
        {
            Quotes = new List<IQuotesData>();
        }
        public List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    class QuotesDalMSSQLSaveQuotesParams : IQuotesDalSaveTimeseriesValuesParams
    {
        public QuotesDalMSSQLSaveQuotesParams()
        {
            Quotes = new List<IQuotesData>();
        }

        
        public List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    class QuotesDalMSSQLSaveQuotesResult : ResultBase, IQuotesDalSaveTimeseriesValuesResult
    {
        public QuotesDalMSSQLSaveQuotesResult()
        {
            TimeSeriesSaved = 0;
        }

        public uint TimeSeriesSaved
        {
            get;
            set;
        }
    }

    class QuotesDalMSSQLGetTimeSeriesInfoParams : IQuotesDalGetTimeSeriesInfoParams
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
    }

    class QuotesDalMSSQLGetTimeSeriesInfoResult : ResultBase, IQuotesDalGetTimeSeriesInfoResult
    {

        public QuotesDalMSSQLGetTimeSeriesInfoResult()
        {
            Series = new List<TimeSeriesInfoListItem>();
            Columns = new List<string>();
        }

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

        public IList<TimeSeriesInfoListItem> Series
        {
            get;
            set;
        }

        public IList<string> Columns
        {
            get;
            set;
        }

        public ETimeSeriesType Type
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

    class QuotesDalMSSQLGetTickersListParams : IQuotesDalGetTickersListParams
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

    class QuotesDalMSSQLGetTickersListResult : ResultBase, IQuotesDalGetTickersListResult
    {
        public QuotesDalMSSQLGetTickersListResult()
        {
            Tickers = new List<TickersListItem>();
        }

        public IList<TickersListItem> Tickers
        {
            get;
            set;
        }
    }
}
