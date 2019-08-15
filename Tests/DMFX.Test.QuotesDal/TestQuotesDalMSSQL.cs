using DMFX.QuotesDAL;
using DMFX.QuotesInterfaces;
using DMFX.Source.Stooq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.QuotesDal
{
    [TestFixture]
    class TestQuotesDalMSSQL
    {
        [SetUp]
        public void Setup()
        {            
        }

        [Test]
        public void GetQuotes_Single_Success()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetQuotesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationManager.AppSettings["CountryUS"];
            getParams.Tickers.Add(ConfigurationManager.AppSettings["TickerSPY"]);
            getParams.TimeFrame = ETimeFrame.Daily;

            IQuotesDalGetQuotesResult result = dal.GetQuotes(getParams);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 1);
            Assert.AreEqual(result.Quotes[0].Ticker, ConfigurationManager.AppSettings["TickerSPY"]);
            Assert.AreEqual(result.Quotes[0].Country, ConfigurationManager.AppSettings["CountryUS"]);
            Assert.AreNotEqual(result.Quotes[0].Quotes.Count(), 0);
        }

        [Test]
        public void GetQuotes_Single_Failed()
        {
            IQuotesDal dal = PrepareQuotesDal();

            IQuotesDalGetQuotesParams getParams = dal.CreateGetQuotesParams();
            getParams.Country = ConfigurationManager.AppSettings["CountryUS"];
            getParams.Tickers.Add(ConfigurationManager.AppSettings["InvalidTicker"]);
            getParams.TimeFrame = ETimeFrame.Daily;

            IQuotesDalGetQuotesResult result = dal.GetQuotes(getParams);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Quotes);
            Assert.AreEqual(result.Quotes.Count, 0);

        }

        [Test]
        public void SaveQuotes_Single_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["CountryUS"];
            getQuotesParams.Ticker = ConfigurationManager.AppSettings["TickerSPY"];
            getQuotesParams.PeriodStart = DateTime.Parse("2009/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/1/1");
            getQuotesParams.TimeFrame = ETimeFrame.Daily;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            IQuotesDal dal = PrepareQuotesDal();
            IQuotesDalSaveQuotesParams saveParams = dal.CreateSaveQuotesParams();
            saveParams.Quotes.Add(getQuotesResult.QuotesData);

            getQuotesResult.QuotesData.Unit = EUnit.USD;
            getQuotesResult.QuotesData.Type = ETimeSeriesType.Price;

            IQuotesDalSaveQuotesResult saveResult = dal.SaveQuotes(saveParams);

            Assert.IsTrue(saveResult.Success);
            Assert.IsTrue(!saveResult.HasWarnings, "Unexpected warnings while performing save");
            Assert.IsTrue(!saveResult.HasErrors, "Unexpected errors while performing save");
            
        }

        [Test]
        public void SaveQuotes_Multiple_Success()
        {
            IQuotesSource source = CreateSource();
            IQuotesDal dal = PrepareQuotesDal();
            IQuotesDalSaveQuotesParams saveParams = dal.CreateSaveQuotesParams();

            string[] tickers = { ConfigurationManager.AppSettings["TickerSPY"], ConfigurationManager.AppSettings["TickerQQQ"] };

            foreach (var t in tickers)
            {

                IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
                getQuotesParams.Country = ConfigurationManager.AppSettings["CountryUS"];
                getQuotesParams.Ticker = t;
                getQuotesParams.PeriodStart = DateTime.Parse("2009/1/1");
                getQuotesParams.PeriodEnd = DateTime.Parse("2019/1/1");
                getQuotesParams.TimeFrame = ETimeFrame.Daily;

                IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

                saveParams.Quotes.Add(getQuotesResult.QuotesData);

                getQuotesResult.QuotesData.Unit = EUnit.USD;
                getQuotesResult.QuotesData.Type = ETimeSeriesType.Price;
            }

            IQuotesDalSaveQuotesResult saveResult = dal.SaveQuotes(saveParams);

            Assert.IsTrue(saveResult.Success);
            Assert.IsTrue(!saveResult.HasWarnings, "Unexpected warnings while performing save");
            Assert.IsTrue(!saveResult.HasErrors, "Unexpected errors while performing save");

        }

        #region Support methods
        IQuotesDal PrepareQuotesDal()
        {
            IQuotesDal dal = new QuotesDalMSSQL();
            IQuotesDalInitParams initParams = dal.CreateInitParams();
            initParams.Parameters["ConnectionStringTimeSeries"] = ConfigurationManager.AppSettings["ConnectionStringMSSQLQuotes"];

            dal.Init(initParams);

            return dal;
        }

        IQuotesSource CreateSource()
        {
            IQuotesSource source = new StooqSource();

            IQuotesSourceInitParams initParams = source.CreateInitParams();
            source.Init(initParams);

            return source;
        }
        #endregion
    }
}
