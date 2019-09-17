using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.CFTC.Api
{
    public class CFTCRecord
    {
        List<decimal> _values = null;

        public CFTCRecord(int records)
        {
            _values = new List<decimal>(new decimal[records]);
        }

        public DateTime ReportDate
        {
            get;
            set;
        }

        public decimal this[int i]
        {
            get
            {
                return _values[i];
            }
            set
            {
                _values[i] = value;
            }
        }

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        public IList<decimal> Values
        {
            get
            {
                return _values;
            }
        }
        

        public override string ToString()
        {
            string str = ReportDate.ToShortDateString() + " ";
            for (int i = 0; i < _values.Count; ++i)
            {
                str += _values[i].ToString("0.00") + (i + 1 < _values.Count ? " " : string.Empty);
            }

            return str;
        }
    }

    public class CFTCInstrumentQuotes
    {
        public CFTCInstrumentQuotes()
        {
            Quotes = new List<CFTCRecord>();
            Timeseries = new List<string>();
        }

        public string Ticker
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public List<CFTCRecord> Quotes
        {
            get;
            set;
        }

        public List<string> Timeseries
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            return Ticker.GetHashCode() * 23 + Description.GetHashCode();
        }

        public override string ToString()
        {
            return Description + " (" + Ticker + ")";
        }
    }
}
