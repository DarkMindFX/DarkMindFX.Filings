using DMFX.QuotesInterfaces;
using DMFX.Source.BLS;
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
    class TestBLSSource
    {
        [Test]
        public void TestBLS_GetSingle_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["BLS_COUNTRY"];
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["BLS_TICKER_CPI_NSA"]);
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
            Assert.IsNotNull(getQuotesResult.QuotesData[0].Metadata, "Metadata was not crated");
            Assert.Greater(getQuotesResult.QuotesData[0].Metadata.Values.Count, 0, "Metadata values were not read");

        }

        [Test]
        public void TestBLS_GetMultiple_Success()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["BLS_COUNTRY"];
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["BLS_TICKER_CPI_NSA"]);
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["BLS_TICKER_CPI_MEDICAL"]);
            getQuotesParams.PeriodStart = DateTime.Parse("2019/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/12/12");
            getQuotesParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            Assert.IsNotNull(getQuotesResult);
            Assert.IsTrue(getQuotesResult.Success);
            Assert.False(getQuotesResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(getQuotesResult.QuotesData, "QuotesData object was not created");
            for (int i = 0; i < getQuotesParams.Tickers.Count; ++i)
            {
                Assert.IsNotNull(getQuotesResult.QuotesData[i].Quotes, string.Format( "{0} - Quotes array was not created", getQuotesParams.Tickers[i]));
                Assert.Greater(getQuotesResult.QuotesData[i].Quotes.Count(), 0, string.Format("{0} - Quotes were not read", getQuotesParams.Tickers[i]));
                Assert.IsNotNull(getQuotesResult.QuotesData[i].Metadata, string.Format("{0} - Metadata was not crated", getQuotesParams.Tickers[i]));
                Assert.Greater(getQuotesResult.QuotesData[i].Metadata.Values.Count, 0, string.Format("{0} - Metadata values were not read", getQuotesParams.Tickers[i]));
            }

        }

        [Test]
        public void TestBLS_GetMultiple_PartialSuccess()
        {
            IQuotesSource source = CreateSource();

            IQuotesSourceGetQuotesParams getQuotesParams = source.CreateGetQuotesParams();
            getQuotesParams.Country = ConfigurationManager.AppSettings["BLS_COUNTRY"];
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["BLS_TICKER_CPI_NSA"]);
            getQuotesParams.Tickers.Add(ConfigurationManager.AppSettings["BLS_TICKER_INVALID"]);
            getQuotesParams.PeriodStart = DateTime.Parse("2019/1/1");
            getQuotesParams.PeriodEnd = DateTime.Parse("2019/12/12");
            getQuotesParams.TimeFrame = ETimeFrame.Monthly;

            IQuotesSourceGetQuotesResult getQuotesResult = source.GetQuotes(getQuotesParams);

            Assert.IsNotNull(getQuotesResult);
            Assert.IsTrue(getQuotesResult.Success);
            Assert.False(getQuotesResult.HasErrors, "Unexpected error occured");
            Assert.IsNotNull(getQuotesResult.QuotesData, "QuotesData object was not created");
            Assert.AreEqual(getQuotesResult.QuotesData.Count(), 1, "Unexpected quotes read");
            Assert.IsNotNull(getQuotesResult.QuotesData[0].Quotes, "Quotes array was not created");
            Assert.Greater(getQuotesResult.QuotesData[0].Quotes.Count(), 0, "Quotes were not read");            
            Assert.IsNotNull(getQuotesResult.QuotesData[0].Metadata, "Metadata was not crated");
            Assert.Greater(getQuotesResult.QuotesData[0].Metadata.Values.Count, 0, "Metadata values were not read");

        }

        #region Support methods
        IQuotesSource CreateSource()
        {
            IQuotesSource source = new BLSSource();

            IQuotesSourceInitParams initParams = source.CreateInitParams();
            source.Init(initParams);

            return source;
        }
        #endregion
    }


}
