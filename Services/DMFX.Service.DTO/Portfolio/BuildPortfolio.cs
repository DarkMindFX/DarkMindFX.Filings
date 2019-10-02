using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{

    [Route("/BuildPortfolio", "POST")]
    public class BuildPortfolio : RequestBase, IReturn<GetTimeSeriesResponse>
    {
        public BuildPortfolio()
        {
            Instruments = new List<string>();
            Constraints = new List<PortfolioBuilderConstraint>();
        }
        public DateTime? PeriodStart
        {
            get;
            set;
        }

        public DateTime? PeriodEnd
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        EProtfolioProperty OptimizationTarget
        {
            get;
            set;
        }

        EOptimizationGoal Goal
        {
            get;
            set;
        }

        public List<string> Instruments
        {
            get;
            set;
        }

        public List<PortfolioBuilderConstraint> Constraints
        {
            get;
            set;
        }
    }

    public class BuildPortfolioResult : ResponseBase
    {
        public decimal Return
        {
            get;
            set;
        }

        public decimal StDev
        {
            get;
            set;
        }

        public Portfolio Portfolio
        {
            get;
            set;
        }
    }
}
