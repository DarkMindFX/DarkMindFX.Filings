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

    public interface IQuotesRecord
    {
        DateTime Time
        {
            get;
            set;
        }

        decimal Open
        {
            get;
            set;
        }

        decimal High
        {
            get;
            set;
        }

        decimal Low
        {
            get;
            set;
        }

        decimal Close
        {
            get;
            set;
        }

        decimal Volume
        {
            get;
            set;
        }

    }

    public class BaseQuotesRecord : IQuotesRecord
    {
        public decimal Close
        {
            get;
            set;
        }

        public decimal High
        {
            get;
            set;
        }

        public decimal Low
        {
            get;
            set;
        }

        public decimal Open
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public decimal Volume
        {
            get;
            set;
        }
    }

    public interface IQuotesData
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

        IEnumerable<IQuotesRecord> Quotes
        {
            get;
            set;
        }

        void AddRecord(IQuotesRecord newRecord);

        IQuotesRecord CreateQuotesRecord();

    }

    public class BaseQuotesData : IQuotesData
    {
        public BaseQuotesData()
        {
            Quotes = new List<IQuotesRecord>();
        }

        public string Country
        {
            get;
            set;
        }

        public IEnumerable<IQuotesRecord> Quotes
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

        public void AddRecord(IQuotesRecord newRecord)
        {
            (Quotes as List<IQuotesRecord>).Add(newRecord);
        }

        public IQuotesRecord CreateQuotesRecord()
        {
            return new BaseQuotesRecord();
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
