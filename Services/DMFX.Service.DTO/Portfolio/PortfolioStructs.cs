using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    public enum ETimeFrame
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Annually
    }

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

    public class PortfolioBuilderConstraint
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
}
