using DMFX.PortfolioInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Portfolio
{
    
    public class Portfolio : IPortfolio
    {

        public Portfolio()
        {
            Instruments = new Dictionary<string, decimal>();
        }

        public Portfolio(IDictionary<string, decimal> instruments)
        {
            Instruments = new Dictionary<string, decimal>(instruments);
        }

        public IDictionary<string, decimal> Instruments
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
