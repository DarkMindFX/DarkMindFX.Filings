using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/SavePortfolio", "POST")]
    public class SavePortfolio : RequestBase, IReturn<SavePortfolioResponse>
    {
        public Portfolio Portfolio
        {
            get;
            set;
        }
    }

    public class SavePortfolioResponse : ResponseBase
    {
    }
}
