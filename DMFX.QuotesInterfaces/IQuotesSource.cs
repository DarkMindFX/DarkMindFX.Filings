﻿using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesInterfaces
{

    public enum ETimeFrame
    {
        Undefined = 0,
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Annually
    }

    public enum EUnit
    {
        Undefined = 1,

        USD = 2,
        EUR = 3,
        Percent = 4,

        Value = 5
    }

    public enum ETimeSeriesType
    {
        Price = 1,
        Indicator = 2
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

    public interface IQuotesSourceCanImportParams
    {
        IList<string> Tickers
        {
            get;
            set;
        }
    }

    public interface IQuotesSourceCanImportResult : IResult
    {
        IList<string> Tickers
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

        /// <summary>
        /// This method is used to check which quotes can be imported. The list of tickers is provided in the parameters
        /// The response will contain the subset of tickers which this source supports
        /// </summary>
        /// <param name="canImportParams"></param>
        /// <returns></returns>
        IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams);

        IQuotesSourceInitParams CreateInitParams();

        IQuotesSourceGetQuotesParams CreateGetQuotesParams();

        IQuotesSourceCanImportParams CreateCanImportParams();

    }
}
