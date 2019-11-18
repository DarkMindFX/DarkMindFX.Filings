using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Source.BLS
{
    public class BLSSourceInitParams : IQuotesSourceInitParams
    {
        public BLSSourceInitParams()
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

    public class BLSSourceGetQuotesParams : IQuotesSourceGetQuotesParams
    {
        public BLSSourceGetQuotesParams()
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

    public class BLSSourceGetQuotesResult : ResultBase, IQuotesSourceGetQuotesResult
    {
        public BLSSourceGetQuotesResult()
        {
            QuotesData = new List<IQuotesData>();
        }
        public IList<IQuotesData> QuotesData
        {
            get;
            set;
        }
    }

    public class BLSSourceCanImportParams : IQuotesSourceCanImportParams
    {
        public BLSSourceCanImportParams()
        {
            Tickers = new List<string>();
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }

    public class BLSSourceCanImportResult : ResultBase, IQuotesSourceCanImportResult
    {
        public BLSSourceCanImportResult()
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
