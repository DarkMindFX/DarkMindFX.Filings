using AutoDiff;
using DMFX.Interfaces;
using DMFX.PortfolioInterfaces;
using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Portfolio
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IPortfolioBuilder))]
    public class PortfolioBuilder : IPortfolioBuilder
    {

        private ILogger _logger;
        private IQuotesDal _quotesDal;

        public void Init(IPortfolioBuilderInitParams initParams)
        {
            _logger = initParams.Logger;
            _quotesDal = initParams.QuotesDal;     

        }

        public IPortfolioBuilderBuildResult Build(IPortfolioBuilderBuildParams buildParams)
        {
            IPortfolioBuilderBuildResult result = null;

            PrepareStatData(buildParams);

            fReturn = PrepareReturnFunction();
            fStDev = PrepareStDevFunction();
            
            //lower bound is 0 - we cannot have negative weights
            double[] bndl = new double[buildParams.Instruments.Count];
            for (int i = 0; i < buildParams.Instruments.Count; ++i)
            {
                string symbol = buildParams.Instruments[i];
                IPortfolioBuilderConstraint constr = buildParams.Constraints.FirstOrDefault(x =>
                    x.Property == EProtfolioProperty.Instrument &&
                    x.Ticker == symbol
                    && (x.Operation == EConstraintOp.Greater || x.Operation == EConstraintOp.GreaterOrEqual || x.Operation == EConstraintOp.Equal));
                bndl[i] = constr != null ? (constr.Value >= 0 ? (double)constr.Value : 0) : 0;
                
            }

            //upper bound 
            double[] bndu = new double[buildParams.Instruments.Count];
            for (int i = 0; i < buildParams.Instruments.Count; ++i)
            {
                string symbol = buildParams.Instruments[i];
                IPortfolioBuilderConstraint constr = buildParams.Constraints.FirstOrDefault(x => 
                    x.Property == EProtfolioProperty.Instrument && 
                    x.Ticker == symbol 
                    && (x.Operation == EConstraintOp.Less || x.Operation == EConstraintOp.LessOrEqual || x.Operation == EConstraintOp.Equal));
                bndu[i] = constr != null ? (constr.Value >= 0 ? (double)constr.Value : 1) : 1;
            }
            
            // constraining weights' sum to 100%
            double[,] c = new double[1, buildParams.Instruments.Count + 1]; //{ 1, 1, 1, 1, 1, 1 }
            for (int i = 0; i < buildParams.Instruments.Count + 1; ++i)
                c[0, i] = 1;
            int[] ct = { 0 };

            // array to hold weights
            double[] w = new double[buildParams.Instruments.Count];

            alglib.minbleicstate state;
            alglib.minbleicreport rep;

            double epsg = 0.000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;

            for (int i = 0; i < buildParams.MaxIterations; ++i)
            {
                GenerateRandomWeights(w);

                alglib.minbleiccreate(w, out state);
                alglib.minbleicsetbc(state, bndl, bndu);
                alglib.minbleicsetlc(state, c, ct);
                alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);

                switch (buildParams.OptimizationTarget)
                {
                    case EProtfolioProperty.Return:
                        alglib.minbleicoptimize(state, FunReturn, null, null);
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Optimization for {0} is not supported", buildParams.OptimizationTarget.ToString()));
                }

                alglib.minbleicresults(state, out w, out rep);

                
            }

            TPortfolio bestPortfolio = null;

            switch (buildParams.OptimizationTarget)
            {
                case EProtfolioProperty.Return:
                    IPortfolioBuilderConstraint stDevConstr = buildParams.Constraints.FirstOrDefault(x => x.Property == EProtfolioProperty.StDev);
                    
                    IEnumerable<TPortfolio> orderedPortfolios = null;
                    switch (buildParams.Goal)
                    {
                        case EOptimizationGoal.Max:
                            orderedPortfolios = portfolios.OrderByDescending(x => x.Return);
                            break;
                        case EOptimizationGoal.Min:
                            orderedPortfolios = portfolios.OrderBy(x => x.Return);
                            break;
                    }
                    if (stDevConstr != null)
                    {
                        switch (stDevConstr.Operation)
                        {
                            case EConstraintOp.Greater:
                                orderedPortfolios = orderedPortfolios.Where(x => x.StDev > (double)stDevConstr.Value);
                                break;
                            case EConstraintOp.Equal:
                                orderedPortfolios = orderedPortfolios.Where(x => Double.Equals(x.StDev, (double)stDevConstr.Value));
                                break;
                            case EConstraintOp.GreaterOrEqual:
                                orderedPortfolios = orderedPortfolios.Where(x => x.StDev >= (double)stDevConstr.Value);
                                break;
                            case EConstraintOp.Less:
                                orderedPortfolios = orderedPortfolios.Where(x => x.StDev < (double)stDevConstr.Value);
                                break;
                            case EConstraintOp.LessOrEqual:
                                orderedPortfolios = orderedPortfolios.Where(x => x.StDev <= (double)stDevConstr.Value);
                                break;
                        }
                    }
                    bestPortfolio = orderedPortfolios.FirstOrDefault();
                    break;
            }

            // getting result
            
            result = new PortfolioBuilderBuildResult();
            if (bestPortfolio != null)
            {

                result.Success = true;
                result.Portfolio = new Portfolio();
                for (int i = 0; i < bestPortfolio.Weights.Count; ++i)
                {
                    result.Portfolio.Instruments.Add(buildParams.Instruments[i], Math.Round( (decimal)bestPortfolio.Weights[i], 4 ));                    
                }
                result.Return = (decimal)bestPortfolio.Return;
                result.StDev = (decimal)bestPortfolio.StDev;
            }
            else
            {
                result.Success = false;
            }

            return result;
        }

        public IPortfolioBuilderConstraint CreateConstraint()
        {
            return new PortfolioBuilderConstraint();
        }

        public IPortfolioBuilderInitParams CreateInitParams()
        {
            return new PortfolioBuilderInitParams();
        }

        public IPortfolioBuilderBuildParams CreatePortfolioBuilderBuildParams()
        {
            return new PortfolioBuilderBuildParams();
        }

        
        #region Support methods

        class TPortfolio
        {
            public TPortfolio()
            {
                Weights = new List<double>();
            }
            public List<double> Weights
            {
                get;
                set;
            }

            public double Return
            {
                get;
                set;
            }

            public double StDev
            {
                get;
                set;
            }
            
        }

        Term fReturn = null;
        Term fStDev = null;
        List<Term> fReturnTerms = new List<Term>();
        Dictionary<int, Variable> variables = new Dictionary<int, Variable>();
        List<TPortfolio> portfolios = new List<TPortfolio>();

        double[,] matCov = null;

        double[] stDevs = null;

        double[] rets = null;

        private Term PrepareReturnFunction(int count = 5)
        {
            Term t = null;
            for (int i = 0; i < count; ++i)
            {

                Variable w = new Variable();

                Term part = rets[i] * w;

                variables.Add(i, w);
                fReturnTerms.Add(part);
                if (t == null)
                {
                    t = part;
                }
                else
                {
                    t += part;
                }
            }

            t = t * -1; // to use minimization

            return t;
        }

        private Term PrepareStDevFunction(int count = 5)
        {
            Term stDev = null;

            for (int i = 0; i < count; ++i)
            {
                Variable w = new Variable();
                if (!variables.ContainsKey(i))
                {
                    variables.Add(i, w);
                }
                var std_mul_w = TermBuilder.Power(variables[i], 2) * TermBuilder.Power(stDevs[i], 2);
                if (i == 0)
                {
                    stDev = std_mul_w;
                }
                else
                {
                    stDev += std_mul_w;
                }
                for (int k = i + 1; k < count; ++k)
                {
                    Variable w2 = new Variable();
                    if (!variables.ContainsKey(k))
                    {
                        variables.Add(k, w2);
                    }
                    var cov = 2 * matCov[i, k] * variables[i] * variables[k];
                    stDev += cov;
                }
            }

            stDev = TermBuilder.Power(stDev, 0.5);

            return stDev;
        }

        private void FunReturn(double[] weights, ref double func, double[] grad, object obj)
        {

            func = fReturn.Evaluate(variables.Values.ToArray(), weights.ToArray());
            double stdev = fStDev.Evaluate(variables.Values.ToArray(), weights.ToArray());

            TPortfolio newPortfolio = new TPortfolio();
            newPortfolio.Weights.AddRange(weights);
            newPortfolio.Return = func * -1;
            newPortfolio.StDev = stdev;

            portfolios.Add(newPortfolio);

            var gradient = fReturn.Differentiate(variables.Values.ToArray(), weights.ToArray());
            for (int g = 0; g < gradient.Count(); ++g)
            {
                grad[g] = gradient[g];
            }

        }

        private void GenerateRandomWeights(double[] weights, int count = 5)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            double sum = 0;
            for (int i = 0; i < count && sum < 1; ++i)
            {
                double w = Math.Round(rnd.NextDouble() / 3, 1);
                if (w + sum > 1)
                {
                    w = 1 - sum;
                }
                weights[i] = w;
                sum += w;
            }
        }

        private void PrepareStatData(IPortfolioBuilderBuildParams builderParams)
        {
            int nInstruments = builderParams.Instruments.Count;
            rets = new double[nInstruments];
            stDevs = new double[nInstruments];
            matCov = new double[nInstruments, nInstruments];

            // getting instruments and calculating avg returns and stdevs for given time period
            IQuotesDalGetTimeSeriesValuesParams getQuotesParams = _quotesDal.CreateGetQuotesParams();
            getQuotesParams.PeriodStart = builderParams.PeriodStart;
            getQuotesParams.PeriodEnd = builderParams.PeriodEnd;
            getQuotesParams.TimeFrame = (QuotesInterfaces.ETimeFrame)builderParams.TimeFrame;
            getQuotesParams.Tickers.AddRange( builderParams.Instruments );

            IQuotesDalGetTimeseriesValuesResult getQuotesResult = _quotesDal.GetTimseriesValues(getQuotesParams);

            Dictionary<string, List<double>> returns = new Dictionary<string, List<double>>();

            // calculating returns & stdev
            for (int i = 0; i < nInstruments; ++i)
            {
                double avgRet = 0;
                returns.Add(getQuotesResult.Quotes[i].Ticker, new List<double>());
                for (int d = 0; d < getQuotesResult.Quotes[i].Quotes.Count(); ++d)
                {
                    double ret = (double)(getQuotesResult.Quotes[i].Quotes[d]["Close"] - getQuotesResult.Quotes[i].Quotes[d]["Open"]) / (double)getQuotesResult.Quotes[i].Quotes[d]["Open"];
                    returns[getQuotesResult.Quotes[i].Ticker].Add(ret);
                    avgRet += ret;
                }

                avgRet /= getQuotesResult.Quotes[i].Quotes.Count() - 1;
                switch (builderParams.TimeFrame)
                {
                    case PortfolioInterfaces.ETimeFrame.Monthly:
                        avgRet = Math.Pow( 1+ avgRet, 12) - 1;
                        break;
                }
                rets[i] = avgRet;
                stDevs[i] = MathNet.Numerics.Statistics.Statistics.PopulationStandardDeviation(returns[getQuotesResult.Quotes[i].Ticker]);
                switch (builderParams.TimeFrame)
                {
                    case PortfolioInterfaces.ETimeFrame.Monthly:
                        stDevs[i] *= Math.Sqrt(12);
                        break;
                }
            }

            // calculating covariances
            for (int k = 0; k < returns.Keys.Count; ++k)
            {
                for (int o = k; o < returns.Keys.Count; ++o)
                {
                    matCov[k, o] = MathNet.Numerics.Statistics.Statistics.PopulationCovariance(returns[returns.Keys.ElementAt(k)], returns[returns.Keys.ElementAt(o)]);
                    matCov[o, k] = matCov[k, o];
                }
            }       
        }
        #endregion
    }
}
