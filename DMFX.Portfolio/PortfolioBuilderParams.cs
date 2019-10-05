using DMFX.PortfolioInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.Interfaces;
using DMFX.QuotesInterfaces;

namespace DMFX.Portfolio
{
    public class PortfolioBuilderConstraint : IPortfolioBuilderConstraint
    {
        public EConstraintOp Operation
        {
            get;
            set;
        }

        public EProtfolioProperty Property
        {
            get;
            set;
        }

        public string Ticker
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
            string result = null;
            switch (Property)
            {
                
                case EProtfolioProperty.Return:
                    result = "Return " + Operation.ToString() + " " + Value;
                    break;
                case EProtfolioProperty.StDev:
                    result = "StDev " + Operation.ToString() + " " + Value;
                    break;
                case EProtfolioProperty.SharpeRatio:
                    result = "SharpeRatio " + Operation.ToString() + " " + Value;
                    break;
                default:
                    result = Ticker + " " + Operation.ToString() + " " + Value;
                    break;
            }
            return result;
        }
    }

    public class PortfolioBuilderInitParams : IPortfolioBuilderInitParams
    {
        public ILogger Logger
        {
            get;
            set;
        }

        public IQuotesDal QuotesDal
        {
            get;
            set;
        }
    }
    
    public class PortfolioBuilderBuildParams : IPortfolioBuilderBuildParams
    {
        public PortfolioBuilderBuildParams()
        {
            Constraints = new List<IPortfolioBuilderConstraint>();
            MaxIterations = 1000;
            TimeFrame = PortfolioInterfaces.ETimeFrame.Monthly;
            Instruments = new List<string>();
        }

        public IList<IPortfolioBuilderConstraint> Constraints
        {
            get;
            set;
        }

        public EOptimizationGoal Goal
        {
            get;
            set;
        }

        public IList<string> Instruments
        {
            get;
            set;
        }

        public int MaxIterations
        {
            get;
            set;
        }

        public EProtfolioProperty OptimizationTarget
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public PortfolioInterfaces.ETimeFrame TimeFrame
        {
            get;
            set;
        }

    }

    public class PortfolioBuilderBuildResult : ResultBase, IPortfolioBuilderBuildResult
    {
        public IPortfolio Portfolio
        {
            get;
            set;
        }

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
    }

}
