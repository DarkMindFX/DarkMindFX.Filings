using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DMFX.BLS.Api.BLSApi;

namespace DMFX.BLS.Api
{
    public class CSVRecord
    {
        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }
        

        public decimal Value
        {
            get;
            set;
        }

        public override string ToString()
        {
            return PeriodStart.ToShortDateString() + " - " + PeriodEnd.ToShortDateString() + " " + Value;
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

        public ETimeFrame Timeframe
        {
            get;
            set;
        }
    }
}
