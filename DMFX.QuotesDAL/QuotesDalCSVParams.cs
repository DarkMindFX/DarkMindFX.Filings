using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesDAL
{
    class QuotesDalCSVInitParams : IQuotesDalInitParams
    {
        public QuotesDalCSVInitParams()
        {
            Parameters = new Dictionary<string, string>();

            Parameters["RootFolder"] = string.Empty;
        }

        public Dictionary<string, string> Parameters
        {
            get;
            set;
        }
    }

    class QuotesDalCSVGetQuotesParams : IQuotesDalGetQuotesParams
    {
        public QuotesDalCSVGetQuotesParams()
        {
            Country = "US";
            PeriodStart = DateTime.Now;
            PeriodEnd = DateTime.Parse("1990/1/1");
            TimeFrame = ETimeFrame.Monthly;

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

    class QuotesDalCSVGetQuotesResult : ResultBase, IQuotesDalGetQuotesResult
    {
        public QuotesDalCSVGetQuotesResult()
        {
            Quotes = new List<IQuotesData>();
        }
        public List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    class QuotesDalCSVSaveQuotesParams : IQuotesDalSaveQuotesParams
    {
        public QuotesDalCSVSaveQuotesParams()
        {
            Quotes = new List<IQuotesData>();
        }

        public List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    class QuotesDalCSVSaveQuotesResult : ResultBase, IQuotesDalSaveQuotesResult
    {
    }
}
