using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Stooq.Api
{
    public class CSVRecord
    {
        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public decimal Open
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

        public decimal Close
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
            return PeriodEnd.ToShortDateString() + " " + Open + " " + High + " " + Low + " " + Close;
        }
    }

    public class CSVQuotes
    {
        public string Ticker
        {
            get;
            set;
        }

        public List<CSVRecord> Quotes
        {
            get;
            set;
        }
    }
}
