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

    public interface ITimeSeriesMetadata
    {
        IDictionary<string, string> Values
        {
            get;
            set;
        }
    }

    public class BaseTimeSeriesMetadata : ITimeSeriesMetadata
    {
        Dictionary<string, string> _metadata;

        public BaseTimeSeriesMetadata()
        {
            _metadata = new Dictionary<string, string>();
        }

        public BaseTimeSeriesMetadata(IDictionary<string, string> values)
        {
            _metadata = new Dictionary<string, string>();
            Values = values;
        }

        public IDictionary<string, string> Values
        {
            get
            {
                return _metadata;
            }
            set
            {
                _metadata.Clear();
                foreach (var k in value.Keys)
                {
                    _metadata.Add(k, value[k]);
                }
            }
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

    public class CustomTimeseriesRecord : ITimeSeriesRecord
    {
        public CustomTimeseriesRecord(IList<string> columnNames, DateTime time, IList<decimal> values = null)
        {
            if (values != null && columnNames.Count != values.Count)
            {
                throw new ArgumentException("Mismatch between column names and values counts");
            }
            Time = time;
            var valNames = new List<string>();
            valNames.AddRange(columnNames);
            ValueNames = valNames;

            Values = new List<decimal>(values != null ? values : new decimal[ValueNames.Count]);            
        }

        public CustomTimeseriesRecord(string columnName, DateTime time, decimal? value = null)
        {
            
            Time = time;
            var valNames = new List<string>();
            valNames.Add(columnName);
            ValueNames = valNames;

            Values = new List<decimal>();
            Values.Add(value != null ? (decimal)value : 0);
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
            string result = Time.ToShortDateString() + " ";
            for (int i = 0; i < Values.Count; ++i)
            {
                result += Values[i] + (i + 1 < Values.Count ? " " : string.Empty);
            }

            return result;
        }        

    }


    public interface IQuotesData
    {
        string Ticker
        {
            get;
            set;
        }

        string Name
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

        string AgencyCode
        {
            get;
            set;
        }

        string Notes
        {
            get;
            set;
        }

        EUnit Unit
        {
            get;
            set;
        }

        ETimeSeriesType Type
        {
            get;
            set;
        }

        List<ITimeSeriesRecord> Quotes
        {
            get;
            set;
        }

        ITimeSeriesMetadata Metadata
        {
            get;
            set;
        }

        void AddRecord(ITimeSeriesRecord newRecord);

        ITimeSeriesRecord CreateQuotesRecord();

        ITimeSeriesMetadata CreateQuotesMetadata();

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

        public string AgencyCode
        {
            get;
            set;
        }

        public string Notes
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

        public ETimeSeriesType Type
        {
            get;
            set;
        }

        public ETimeFrame TimeFrame
        {
            get;
            set;
        }

        public EUnit Unit
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
        public ITimeSeriesMetadata Metadata 
        {
            get;
            set;
        }

        public void AddRecord(ITimeSeriesRecord newRecord)
        {
            (Quotes as List<ITimeSeriesRecord>).Add(newRecord);
        }

        public ITimeSeriesMetadata CreateQuotesMetadata()
        {
            return new BaseTimeSeriesMetadata();
        }

        public ITimeSeriesRecord CreateQuotesRecord()
        {
            return new BaseQuotesRecord();
        }
    }
}
