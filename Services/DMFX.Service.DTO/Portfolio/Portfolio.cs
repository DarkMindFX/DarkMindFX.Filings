using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    public class Portfolio
    {
        public string Name
        {
            get;
            set;
        }

        public string[] Instruments
        {
            get;
            set;
        }

        public decimal[] Weights
        {
            get;
            set;
        }
    }
}
