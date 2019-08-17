using DMFX.QuotesInterfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.QuotesDal
{
    [TestFixture]
    public class TestQuotesDalCSV
    {
        string csvRoot = null;

        [SetUp]
        public void Setup()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;

            csvRoot = Path.Combine(dir, "..", ConfigurationSettings.AppSettings["CSVDalRootFolder"]);
        }

        [Test]
        public void GetQuotes_Single_Success()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetTimeSeriesValuesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationSettings.AppSettings["CountryUS"];
            getParams.Tickers.Add( ConfigurationSettings.AppSettings["TickerSPY"] );
            getParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesDalGetTimeseriesValuesResult result = dal.GetTimseriesValues(getParams);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 1);
            Assert.AreEqual(result.Quotes[0].Ticker, ConfigurationSettings.AppSettings["TickerSPY"]);
            Assert.AreEqual(result.Quotes[0].Country, ConfigurationSettings.AppSettings["CountryUS"]);
            Assert.AreNotEqual(result.Quotes[0].Quotes.Count(), 0);
        }

        [Test]
        public void GetQuotes_Single_Failed()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetTimeSeriesValuesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationSettings.AppSettings["CountryUS"];
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["InvalidTicker"]);
            getParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesDalGetTimeseriesValuesResult result = dal.GetTimseriesValues(getParams);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 0);
           
        }

        [Test]
        public void GetQuotes_Multiple_Success()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetTimeSeriesValuesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationSettings.AppSettings["CountryUS"];
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["TickerSPY"]);
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["TickerQQQ"]);
            getParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesDalGetTimeseriesValuesResult result = dal.GetTimseriesValues(getParams);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 2);
            Assert.AreEqual(result.Quotes[0].Ticker, ConfigurationSettings.AppSettings["TickerSPY"]);
            Assert.AreEqual(result.Quotes[0].Country, ConfigurationSettings.AppSettings["CountryUS"]);
            Assert.AreNotEqual(result.Quotes[0].Quotes.Count(), 0);
            Assert.AreEqual(result.Quotes[1].Ticker, ConfigurationSettings.AppSettings["TickerQQQ"]);
            Assert.AreEqual(result.Quotes[1].Country, ConfigurationSettings.AppSettings["CountryUS"]);
            Assert.AreNotEqual(result.Quotes[1].Quotes.Count(), 0);
        }

        [Test]
        public void GetQuotes_Multiple_Fail()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetTimeSeriesValuesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationSettings.AppSettings["CountryUS"];
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["InvalidTicker"]);
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["InvalidTicker"]);
            getParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesDalGetTimeseriesValuesResult result = dal.GetTimseriesValues(getParams);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 0);
        }

        [Test]
        public void GetQuotes_Multiple_Warning()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetTimeSeriesValuesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationSettings.AppSettings["CountryUS"];
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["TickerSPY"]);
            getParams.Tickers.Add(ConfigurationSettings.AppSettings["InvalidTicket"]);
            getParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesDalGetTimeseriesValuesResult result = dal.GetTimseriesValues(getParams);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.HasWarnings);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 1);
            Assert.AreEqual(result.Quotes[0].Ticker, ConfigurationSettings.AppSettings["TickerSPY"]);
            Assert.AreEqual(result.Quotes[0].Country, ConfigurationSettings.AppSettings["CountryUS"]);
            Assert.AreNotEqual(result.Quotes[0].Quotes.Count(), 0);
            
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
        #endregion
    }
}
