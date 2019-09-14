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

    public class CFTCSourceGetQuotesResult : ResultBase, IQuotesSourceGetQuotesResult
    {
        public CFTCSourceGetQuotesResult()
        {
            QuotesData = new BaseQuotesData();
        }
        public IQuotesData QuotesData
        {
            get;
            set;
        }
    }
}
