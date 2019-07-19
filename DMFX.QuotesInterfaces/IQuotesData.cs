using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesInterfaces
{
    public interface ITimeSeriesRecord
    {
        DateTime Time
        {
            get;
            set;
        }

        IList<decimal> Values
        {
            get;
            set;
        }

        IList<string> ValueNames
        {
            get;
            set;
        } 

        decimal this[string column]
        {
            get;
            set;
        }

        decimal this[int i]
        {
            get;
            set;
        }

    }

    public class BaseQuotesRecord : ITimeSeriesRecord
    {
        public BaseQuotesRecord()
        {
            ValueNames = new List<string>();
            ValueNames.Add("Open");
            ValueNames.Add("High");
            ValueNames.Add("Low");
            ValueNames.Add("Close");
            ValueNames.Add("Volume");

            Values = new List<decimal>(new decimal[ValueNames.Count]);
        }

        
        public DateTime Time
        {
            get;
            set;
        }

        public IList<string> ValueNames
        {
            get;
            set;
        }

        public IList<decimal> Values
        {
            get;
            set;
        }        

        public decimal this[string column]
        {
            get
            {
                int index = ValueNames.IndexOf(column);
                return this[index];
            }
            set
            {
                int index = ValueNames.IndexOf(column);
                this[index] = value;
            }
        }

        public decimal this[int i]
        {
            get
            {
                return Values[i];
            }
            set
            {
                Values[i] = value;
            }
        }

        public override string ToString()
        {
            return Time.ToShortDateString() + " " + Open.ToString() + " " + High.ToString() + " " + Low.ToString() + " " + Close.ToString();
        }

        public decimal Open
        {
            get
            {
                return Values[0];
            }
        }

        public decimal High
        {
            get
            {
                return Values[1];
            }
        }

        public decimal Low
        {
            get
            {
                return Values[2];
            }
        }

        public decimal Close
        {
            get
            {
                return Values[3];
            }
        }

        public decimal Volume
        {
            get
            {
                return Values[4];
            }
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

        List<ITimeSeriesRecord> Quotes
        {
            get;
            set;
        }

        void AddRecord(ITimeSeriesRecord newRecord);

        ITimeSeriesRecord CreateQuotesRecord();

    }

    public class BaseQuotesData : IQuotesData
    {
        public BaseQuotesData()
        {
            Quotes = new List<ITimeSeriesRecord>();
            Country = "US";
        }

        public string Country
        {
            get;
            set;
        }

        public List<ITimeSeriesRecord> Quotes
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

        public void AddRecord(ITimeSeriesRecord newRecord)
        {
            (Quotes as List<ITimeSeriesRecord>).Add(newRecord);
        }

        public ITimeSeriesRecord CreateQuotesRecord()
        {
            return new BaseQuotesRecord();
        }
    }
}
