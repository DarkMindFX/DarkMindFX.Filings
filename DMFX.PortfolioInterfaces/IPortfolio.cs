using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.PortfolioInterfaces
{
    public interface IPortfolio
    {
        string Name
        {
            get;
            set;
        }
        IDictionary<string, decimal> Instruments
        {
            get;
            set;
        }
    }
}
