using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    public class TickerQuotes
    {
        

        public TickerQuotes()
        {
            TimePeriod = ETimeFrame.Monthly;
            Quotes = new List<QuoteRecord>();
        }

        public string Code
        {
            get;
            set;
        }

        public ETimeFrame TimePeriod
        {
            get;
            set;
        }

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

        public List<QuoteRecord> Quotes
        {
            get;
            set;
        }
    }

    public class QuoteRecord
    {
        public DateTime Time
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

        public decimal AdjClose
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
}
