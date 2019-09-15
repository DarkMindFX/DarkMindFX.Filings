using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Source.Stooq
{
    public class StooqSourceInitParams : IQuotesSourceInitParams
    {
        public StooqSourceInitParams()
        {
            ExtractFromStorage = false;
        }

        public bool ExtractFromStorage
        {
            get;
            set;
        }

        public ILogger Logger
        {
            get;
            set;
        }

        public IQuotesStorage Storage
        {
            get;

            set;
        }
    }

    public class StooqSourceGetQuotesParams : IQuotesSourceGetQuotesParams
    {
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

        public string Ticker
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

    public class StooqSourceGetQuotesResult : ResultBase, IQuotesSourceGetQuotesResult
    {
        public StooqSourceGetQuotesResult()
        {
            QuotesData = new BaseQuotesData();
        }
        public IQuotesData QuotesData
        {
            get;
            set;
        }
    }

    public class StooqSourceCanImportParams : IQuotesSourceCanImportParams
    {
        public StooqSourceCanImportParams()
        {
            Tickers = new List<string>();
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }

    public class StooqSourceCanImportResult : ResultBase, IQuotesSourceCanImportResult
    {
        public StooqSourceCanImportResult()
        {
            Tickers = new List<string>();
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }
}
