using DMFX.Interfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.PortfolioInterfaces
{
    public enum EProtfolioProperty
    {
        
        Return = 1,
        StDev = 2,
        SharpeRatio = 3,
        Instrument = 100
    }

    public enum EOptimizationGoal
    {
        Min,
        Max
    }

    public enum EConstraintOp
    {
        Equal,
        Less,
        Greater,
        LessOrEqual,
        GreaterOrEqual
    }

    public interface IPortfolioBuilderInitParams
    {
        ILogger Logger
        {
            get;
            set;
        }

        IQuotesDal QuotesDal
        {
            get;
            set;
        }
    }

    public interface IPortfolioBuilderConstraint
    {
        EProtfolioProperty Property
        {
            get;
            set;
        }

        string Ticker
        {
            get;
            set;
        }

        EConstraintOp Operation
        {
            get;
            set;
        }

        decimal Value
        {
            get;
            set;
        }
    }

    public interface IPortfolioBuilderBuildParams
    {
                
        DateTime PeriodStart
        {
            get;
            set;
        }

        DateTime PeriodEnd
        {
            get;
            set;
        }

        ETimeFrame TimeFrame
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

        IList<string> Instruments
        {
            get;
            set;
        }

        IList<IPortfolioBuilderConstraint> Constraints
        {
            get;
            set;
        }
        
        int MaxIterations
        {
            get;
            set;
        }       

    }

    public interface IPortfolioBuilderBuildResult : IResult
    {
        IPortfolio Portfolio
        {
            get;
            set;
        }

        decimal Return
        {
            get;
            set;
        }

        decimal StDev
        {
            get;
            set;
        }
    }

    public interface IPortfolioBuilder
    {
        /// <summary>
        /// Method to init portfolio builder
        /// </summary>
        /// <param name="initParams"></param>
        void Init(IPortfolioBuilderInitParams initParams);

        /// <summary>
        /// Method to build optimal portfolio using specified instruments and constraints
        /// </summary>
        /// <param name="buildParams"></param>
        /// <returns></returns>
        IPortfolioBuilderBuildResult Build(IPortfolioBuilderBuildParams buildParams);

        IPortfolioBuilderInitParams CreateInitParams();

        IPortfolioBuilderConstraint CreateConstraint();

        IPortfolioBuilderBuildParams CreatePortfolioBuilderBuildParams();        
    }
}
