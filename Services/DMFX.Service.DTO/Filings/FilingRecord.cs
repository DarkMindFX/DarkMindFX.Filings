using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    public class FilingRecord
    {
        public string Code
        {
            get;
            set;
        }

        public decimal Value
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

        public string UnitName
        {
            get;
            set;
        }
    }
}
