using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.PortfolioInterfaces
{
    public enum ETimeFrame
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Annually
    }

    public interface IPortfolioCalculatorInitParams
    {
        ILogger Logger
        {
            get;
            set;
        }
    }

    public interface IPortfolioCalculatorCalcPerformanceParams
    {
        IPortfolio Portfolio
        {
            get;
            set;
        }
    }

    public interface IPortfolioPerformanceBreakdownRecord
    {
        DateTime Time
        {
            get;
            set;
        }

        IEnumerable<decimal> Values
        {
            get;
            set;
        }
    }

    public interface IPortfolioPerformanceBreakdown
    {

        IDictionary<DateTime, IPortfolioPerformanceBreakdownRecord> Records
        {
            get;
            set;
        }
    }

    public interface IPortfolioCalculatorCalcPerformanceResult : IResult
    {
        IDictionary<EProtfolioProperty, decimal> PortfolioParameters
        {
            get;
        }

        
    }

    public interface IPortfolioCalculator
    {
        void Init(IPortfolioCalculatorInitParams initParams);

        IPortfolioCalculatorCalcPerformanceResult CalcPerformance(IPortfolioCalculatorCalcPerformanceParams calcPerfParams);
    }
}
