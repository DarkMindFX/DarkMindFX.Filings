using DMFX.QuotesInterfaces;
using DMFX.Source.CFTC;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Sources
{
    [TestFixture]
    class TestCFTCSource
    {
        [Test]
        public void TestCFTC_GetFinFut_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["CFTC_COUNTRY"];
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_FINFUT"]);
            getQuotesParams.PeriodStart = DateTime.Parse("2019/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/12/12");
            getQuotesParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            Assert.IsNotNull(getQuotesResult);
            Assert.IsTrue(getQuotesResult.Success);
            Assert.False(getQuotesResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(getQuotesResult.QuotesData, "QuotesData object was not created");
            Assert.IsNotNull(getQuotesResult.QuotesData[0].Quotes, "Quotes array was not created");
            Assert.Greater(getQuotesResult.QuotesData[0].Quotes.Count(), 0, "Quotes were not read");

        }

        [Test]
        public void TestCFTC_GetQuotesMonthly_InvalidTicker()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["CFTC_COUNTRY"];
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_INVALID"]);
            getQuotesParams.PeriodStart = DateTime.Parse("2009/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/1/1");
            getQuotesParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            Assert.IsNotNull(getQuotesResult);
            Assert.IsFalse(getQuotesResult.Success);
            Assert.True(getQuotesResult.HasErrors, "No errors reported");
            Assert.IsNotNull(getQuotesResult.QuotesData, "QuotesData object was not created");
            Assert.AreEqual(getQuotesResult.QuotesData.Count(), 0, "Quotes object is not empty");

        }

        [Test]
        public void TestCFTC_CanImport_Single_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceCanImportParams canImportParams = source.CreateCanImportParams();

            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_FINFUT"]);


            IQuotesSourceCanImportResult canImportResult = source.CanImport(canImportParams);

            Assert.IsNotNull(canImportResult);
            Assert.IsTrue(canImportResult.Success);
            Assert.False(canImportResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(canImportResult.Tickers);
            Assert.AreEqual(canImportResult.Tickers.Count, 1, "Invalid number of tickers returned");
            Assert.AreEqual(canImportResult.Tickers[0], ConfigurationManager.AppSettings["SEC_AAPL_CODE"], "Expected tickers were not returned");

        }

        [Test]
        public void TestCFTC_CanImport_Single_Fail()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceCanImportParams canImportParams = source.CreateCanImportParams();

            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_INVALID"]);


            IQuotesSourceCanImportResult canImportResult = source.CanImport(canImportParams);

            Assert.IsNotNull(canImportResult);
            Assert.IsFalse(canImportResult.Success);
            Assert.False(canImportResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(canImportResult.Tickers);
            Assert.AreEqual(canImportResult.Tickers.Count, 0, "Invalid number of tickers returned");


        }

        [Test]
        public void TestCFTC_CanImport_Multiple_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceCanImportParams canImportParams = source.CreateCanImportParams();

            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_FINFUT"]);
            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_FINFUTOPT"]);
            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_CMDTSFUT"]);
            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_CMDTSFUTOPT"]);


            IQuotesSourceCanImportResult canImportResult = source.CanImport(canImportParams);

            Assert.IsNotNull(canImportResult);
            Assert.IsTrue(canImportResult.Success);
            Assert.False(canImportResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(canImportResult.Tickers);
            Assert.AreEqual(canImportResult.Tickers.Count, 4, "Invalid number of tickers returned");
            
        }

        [Test]
        public void TestCFTC_CanImport_Multiple_PartialSuccess()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceCanImportParams canImportParams = source.CreateCanImportParams();

            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_COT_FINFUT"]);
            canImportParams.Tickers.Add(ConfigurationManager.AppSettings["CFTC_TICKER_INVALID"]);


            IQuotesSourceCanImportResult canImportResult = source.CanImport(canImportParams);

            Assert.IsNotNull(canImportResult);
            Assert.IsTrue(canImportResult.Success);
            Assert.False(canImportResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(canImportResult.Tickers);
            Assert.AreEqual(canImportResult.Tickers.Count, 1, "Invalid number of tickers returned");
            Assert.Contains(ConfigurationManager.AppSettings["CFTC_TICKER_COT_FINFUT"], canImportResult.Tickers.ToArray(), "Expected ticker was not returned");

        }

        #region Support methods
        IQuotesSource CreateSource()
        {
            IQuotesSource source = new CFTCSource();

            IQuotesSourceInitParams initParams = source.CreateInitParams();
            source.Init(initParams);

            return source;
        }
        #endregion
    }
}
