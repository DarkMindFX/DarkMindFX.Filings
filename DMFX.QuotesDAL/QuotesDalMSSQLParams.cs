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

    class QuotesDalMSSQLGetQuotesParams : IQuotesDalGetQuotesParams
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

    class QuotesDalMSSQLGetQuotesResult : ResultBase, IQuotesDalGetQuotesResult
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

    class QuotesDalMSSQLSaveQuotesParams : IQuotesDalSaveQuotesParams
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

    class QuotesDalMSSQLSaveQuotesResult : ResultBase, IQuotesDalSaveQuotesResult
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
}
