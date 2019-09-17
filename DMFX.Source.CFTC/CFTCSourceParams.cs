using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Source.CFTC
{
    public class CFTCSourceInitParams : IQuotesSourceInitParams
    {
        public CFTCSourceInitParams()
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

    public class CFTCSourceGetQuotesParams : IQuotesSourceGetQuotesParams
    {
        public CFTCSourceGetQuotesParams()
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

    public class CFTCSourceGetQuotesResult : ResultBase, IQuotesSourceGetQuotesResult
    {
        public CFTCSourceGetQuotesResult()
        {
            QuotesData = new List<IQuotesData>();
        }
        public IList<IQuotesData> QuotesData
        {
            get;
            set;
        }
    }

    public class CFTCSourceCanImportParams : IQuotesSourceCanImportParams
    {
        public CFTCSourceCanImportParams()
        {
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }

    public class CFTCSourceCanImportResult : ResultBase, IQuotesSourceCanImportResult
    {
        public CFTCSourceCanImportResult()
        {
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }
}
