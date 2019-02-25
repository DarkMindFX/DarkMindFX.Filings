using DMFX.Portfolio;
using DMFX.PortfolioInterfaces;
using DMFX.QuotesInterfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Portfolio
{
    [TestFixture]
    public class TestPortfolioBuilder
    {
        string csvRoot = null;

        [SetUp]
        public void Setup()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;

            csvRoot = Path.Combine(dir, "..", ConfigurationManager.AppSettings["CSVDalRootFolder"]);
        }

        [Test]
        public void BuildPortfolio_5Tickers_MaxReturn_Success()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IPortfolioBuilder builder = PreparePortfolioBuilder(dal);

            // preparing list of instruments
            IPortfolioBuilderBuildParams buildParams = builder.CreatePortfolioBuilderBuildParams();
            buildParams.Goal = EOptimizationGoal.Max; // maximizing portfolio return
            buildParams.OptimizationTarget = EProtfolioProperty.Return;
            buildParams.Instruments.Add(ConfigurationManager.AppSettings["TickerSPY"]);
            buildParams.Instruments.Add(ConfigurationManager.AppSettings["TickerQQQ"]);
            buildParams.Instruments.Add(ConfigurationManager.AppSettings["TickerGLD"]);
            buildParams.Instruments.Add(ConfigurationManager.AppSettings["TickerSLV"]);
            buildParams.Instruments.Add(ConfigurationManager.AppSettings["TickerTLT"]);

            buildParams.TimeFrame = PortfolioInterfaces.ETimeFrame.Monthly;
            buildParams.PeriodStart = DateTime.Parse(ConfigurationManager.AppSettings["PeriodStart"]);
            buildParams.PeriodEnd = DateTime.Parse(ConfigurationManager.AppSettings["PeriodEnd"]);

            // setting constraints
            foreach (var i in buildParams.Instruments)
            {
                IPortfolioBuilderConstraint cl = builder.CreateConstraint();
                cl.Operation = EConstraintOp.GreaterOrEqual;
                cl.Property = EProtfolioProperty.Instrument;
                cl.Ticker = i;
                cl.Value = 0;

                IPortfolioBuilderConstraint cu = builder.CreateConstraint();
                cu.Operation = EConstraintOp.LessOrEqual;
                cu.Property = EProtfolioProperty.Instrument;
                cu.Ticker = i;
                cu.Value = 1;

                buildParams.Constraints.Add(cl);
                buildParams.Constraints.Add(cu);
            }

            IPortfolioBuilderConstraint cstdev = builder.CreateConstraint();
            cstdev.Operation = EConstraintOp.LessOrEqual;
            cstdev.Property = EProtfolioProperty.StDev;
            cstdev.Value = (decimal)0.1; // limiting stdev to 10%

            buildParams.Constraints.Add(cstdev);

            IPortfolioBuilderBuildResult result = builder.Build(buildParams);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Portfolio);
            Assert.IsNotNull(result.Portfolio.Instruments);
            Assert.AreEqual(result.Portfolio.Instruments.Count, 5);
            Assert.AreEqual(result.Portfolio.Instruments.Values.ElementAt(0) + result.Portfolio.Instruments.Values.ElementAt(1) + result.Portfolio.Instruments.Values.ElementAt(2) + result.Portfolio.Instruments.Values.ElementAt(3) + result.Portfolio.Instruments.Values.ElementAt(4), (decimal)1, "Sum of weights is not 100%");
            

        }

        #region Support methods
        private IQuotesDal PrepareQuotesDal()
        {
            IQuotesDal dal = new QuotesDAL.QuotesDalCSV();

            IQuotesDalInitParams initParams = dal.CreateInitParams();
            initParams.Parameters["RootFolder"] = csvRoot;
            dal.Init(initParams);

            return dal;

        }

        private IPortfolioBuilder PreparePortfolioBuilder(IQuotesDal dal)
        {
            IPortfolioBuilder builder = new PortfolioBuilder();

            // initializing
            IPortfolioBuilderInitParams initParams = builder.CreateInitParams();
            initParams.QuotesDal = dal;
            builder.Init(initParams);

            return builder;

        }
        #endregion
    }
}
