using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesInterfaces
{

    public enum ETimeFrame
    {
        Daily,
        Weekly,
        Monthly
    }

    public interface IQuotesSourceInitParams
    {
        IQuotesStorage Storage
        {
            get;
            set;
        }

        ILogger Logger
        {
            get;
            set;
        }

        bool ExtractFromStorage
        {
            get;
            set;
        }
    }

    public interface IQuotesSourceGetQuotesParams
    {
        string Ticker
        {
            get;
            set;
        }

        string Country
        {
            get;
            set;
        }

        ETimeFrame TimeFrame
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
    }

    public interface IQuotesSourceGetQuotesResult : IResult
    {
        IQuotesData QuotesData
        {
            get;
            set;
        }
    }

    public interface IQuotesSource
    {
        /// <summary>
        /// Method to init quotes source object
        /// </summary>
        /// <param name="initParams"></param>
        void Init(IQuotesSourceInitParams initParams);

        /// <summary>
        /// Method to read get quotes
        /// </summary>
        /// <param name="getQuotesParams"></param>
        /// <returns></returns>
        IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams);

        IQuotesSourceInitParams CreateInitParams();

        IQuotesSourceGetQuotesParams CreateGetQuotesParams();

    }
}
