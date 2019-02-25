using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesInterfaces
{
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

        public override string ToString()
        {
            return Time.ToShortDateString() + " " + Open.ToString() + " " + High.ToString() + " " + Low.ToString() + " " + Close.ToString();
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

        List<IQuotesRecord> Quotes
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

        public List<IQuotesRecord> Quotes
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
}
