using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;

namespace DMFX.Source.BEA
{
    public class BEASourceInitParams : IQuotesSourceInitParams
    {
        public BEASourceInitParams()
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

    public class BEASourceGetQuotesParams : IQuotesSourceGetQuotesParams
    {
        public BEASourceGetQuotesParams()
        {
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

        public IList<string> Tickers
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

    public class BEASourceGetQuotesResult : ResultBase, IQuotesSourceGetQuotesResult
    {
        public BEASourceGetQuotesResult()
        {
            QuotesData = new List<IQuotesData>();
        }
        public IList<IQuotesData> QuotesData
        {
            get;
            set;
        }
    }

    public class BEASourceCanImportParams : IQuotesSourceCanImportParams
    {
        public BEASourceCanImportParams()
        {
            Tickers = new List<string>();
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }

    public class BEASourceCanImportResult : ResultBase, IQuotesSourceCanImportResult
    {
        public BEASourceCanImportResult()
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
