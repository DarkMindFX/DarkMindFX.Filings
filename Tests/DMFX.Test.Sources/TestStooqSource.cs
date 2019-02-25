using DMFX.QuotesInterfaces;
using DMFX.Source.Stooq;
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
    class TestStooqSource
    {
        [Test]
        public void TestStooq_GetQuotesMonthly_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["STOOQ_COUNTRY"];
            getQuotesParams.Ticker = ConfigurationManager.AppSettings["STOOQ_TICKER_SPY"];
            getQuotesParams.PeriodStart = DateTime.Parse("2009/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/1/1");
            getQuotesParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            Assert.IsNotNull(getQuotesResult);
            Assert.IsTrue(getQuotesResult.Success);
            Assert.False(getQuotesResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(getQuotesResult.QuotesData, "QuotesData object was not created");
            Assert.IsNotNull(getQuotesResult.QuotesData.Quotes, "Quotes array was not created");
            Assert.Greater(getQuotesResult.QuotesData.Quotes.Count(), 0, "Quotes were not read");

        }

        [Test]
        public void TestStooq_GetQuotesMonthly_InvalidTicker()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["STOOQ_COUNTRY"];
            getQuotesParams.Ticker = ConfigurationManager.AppSettings["STOOQ_TICKER_INVALID"];
            getQuotesParams.PeriodStart = DateTime.Parse("2009/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/1/1");
            getQuotesParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            Assert.IsNotNull(getQuotesResult);
            Assert.IsFalse(getQuotesResult.Success);
            Assert.True(getQuotesResult.HasErrors, "No errors reported");
            Assert.IsNotNull(getQuotesResult.QuotesData, "QuotesData object was not created");
            Assert.IsNotNull(getQuotesResult.QuotesData.Quotes, "Quotes array was not created");
            Assert.AreEqual(getQuotesResult.QuotesData.Quotes.Count(), 0, "Quotes object is not empty");
            
        }

        #region Support methods
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
