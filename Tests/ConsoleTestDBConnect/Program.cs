using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDiff;


namespace ConsoleTestDBConnect
{
    class Program
    {
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

            public double SharpeRatio
            {
                get;
                set;
            }
        }

        static Term fReturn = null;
        static Term fStDev = null;
        static List<Term> fReturnTerms = new List<Term>();
        static Dictionary<int, Variable> variables = new Dictionary<int, Variable>();
        static List<TPortfolio> portfolios = new List<TPortfolio>();

        static double[,] matCov =
                {
                    { .0017, .0019, .0000, .0007, -.0004 },
                    { .0019, .0025, .0000, .0008, -.0005 },
                    { .0000, .0000, .0027, .0039, .0004 },
                    { .0007, .0008, .0039, .0085, .0001 },
                    { -.0004, -.0005, .0004, .0001, .0014 }
                };

        static double[] stDevs = { 0.1435, 0.1748, 0.1804, 0.3191, 0.1275 };

        static double[] rets = { 0.062, 0.1148, 0.0755, 0.0636, 0.0704 };

        public static Term PrepareReturnFunction(int count = 5)
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

        public static Term PrepareStDevFunction(int count = 5)
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
                    var cov = 2 * matCov[i,k] * variables[i] * variables[k];
                    stDev += cov;
                }
            }

            stDev = TermBuilder.Power(stDev, 0.5);

            return stDev;
        }

        static void FunReturn(double[] weights, ref double func, double[] grad, object obj)
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

        static void GenerateRandomWeights(double[] weights, int count = 5)
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

        static void Main(string[] args)
        {
            try
            {
                fReturn = PrepareReturnFunction();
                fStDev = PrepareStDevFunction();

         
                /**********/
                
                double[] bndl = new double[] { 0, 0, 0, 0, 0 };
                double[] bndu = new double[] { +1, +1, +1, +1, +1 };

                double[,] c = { { 1, 1, 1, 1, 1, 1 } };
                int[] ct = { 0 };
                double[] w = new double[5];

                alglib.minbleicstate state;
                alglib.minbleicreport rep;

                double epsg = 0.000001;
                double epsf = 0;
                double epsx = 0;
                int maxits = 0;

                for (int i = 0; i < 100; ++i)
                {
                    GenerateRandomWeights(w);

                    alglib.minbleiccreate(w, out state);
                    alglib.minbleicsetbc(state, bndl, bndu);
                    alglib.minbleicsetlc(state, c, ct);
                    alglib.minbleicsetcond(state, epsg, epsf, epsx, maxits);
                    alglib.minbleicoptimize(state, FunReturn, null, null);

                    alglib.minbleicresults(state, out w, out rep);
                }

                double maxstDev = 0.10;

                TPortfolio bestPortfolio = portfolios.Where(p => p.StDev <= maxstDev).OrderByDescending(x => x.Return).FirstOrDefault();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }
}
