using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DMFX.Source.XYZ
{
    public class XYZSourceInitParams : IQuotesSourceInitParams
    {
        public XYZSourceInitParams()
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

    public class XYZSourceGetQuotesParams : IQuotesSourceGetQuotesParams
    {
        public XYZSourceGetQuotesParams()
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

    public class XYZSourceGetQuotesResult : ResultBase, IQuotesSourceGetQuotesResult
    {
        public XYZSourceGetQuotesResult()
        {
            QuotesData = new List<IQuotesData>();
        }
        public IList<IQuotesData> QuotesData
        {
            get;
            set;
        }
    }

    public class XYZSourceCanImportParams : IQuotesSourceCanImportParams
    {
        public XYZSourceCanImportParams()
        {
            Tickers = new List<string>();
        }

        public IList<string> Tickers
        {
            get;
            set;
        }
    }

    public class XYZSourceCanImportResult : ResultBase, IQuotesSourceCanImportResult
    {
        public XYZSourceCanImportResult()
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
