using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesInterfaces
{
    public interface IQuotesDalInitParams
    {
        Dictionary<string, string> Parameters
        {
            get;
        }
    }

    public interface IQuotesDalGetQuotesParams
    {
        List<string> Tickers
        {
            get;
            set;
        }

        string Country
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

        ETimeFrame TimeFrame
        {
            get;
            set;
        }
    }

    public interface IQuotesDalGetQuotesResult : IResult
    {
        List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    public interface IQuotesDalSaveQuotesParams
    {

        List<IQuotesData> Quotes
        {
            get;
            set;
        }
    }

    public interface IQuotesDalSaveQuotesResult : IResult
    {       
        uint TimeSeriesSaved
        {
            get;
            set;
        } 
    }

    public interface IQuotesDal
    {
        /// <summary>
        /// Method to init DAL
        /// </summary>
        /// <param name="initParams"></param>
        void Init(IQuotesDalInitParams initParams);

        /// <summary>
        /// Returns quotes for a given list of tickers
        /// </summary>
        /// <param name="getQuotesParams"></param>
        /// <returns></returns>
        IQuotesDalGetQuotesResult GetQuotes(IQuotesDalGetQuotesParams getQuotesParams);

        IQuotesDalSaveQuotesResult SaveQuotes(IQuotesDalSaveQuotesParams saveQuotesParams);

        IQuotesDalInitParams CreateInitParams();

        IQuotesDalGetQuotesParams CreateGetQuotesParams();

        IQuotesDalSaveQuotesParams CreateSaveQuotesParams();


    }
}
