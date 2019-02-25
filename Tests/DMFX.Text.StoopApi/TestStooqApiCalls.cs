using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Text.StoopApi
{
    [TestFixture]
    public class TestStooqApiCalls
    {
        [Test]
        public void DownloadMonthly_Success()
        {
            DMFX.Stooq.Api.StooqApi api = new Stooq.Api.StooqApi();

            DMFX.Stooq.Api.CSVQuotes quotes = api.Download("SPY", DateTime.Parse("2005/2/25"), DateTime.Now, "US", Stooq.Api.StooqApi.ETimeFrame.Monthly);

            Assert.IsNotNull(quotes, "Result object was not created");
            Assert.IsNotEmpty(quotes.Ticker, "Ticker field is not set");
            Assert.IsNotNull(quotes.Quotes, "Quotes array is not created");
            Assert.AreNotEqual(quotes.Quotes.Count, 0, "SPY quotes were not retrieved");           
        }

        [Test]
        public void DownloadWeekly_Success()
        {
            DMFX.Stooq.Api.StooqApi api = new Stooq.Api.StooqApi();

            DMFX.Stooq.Api.CSVQuotes quotes = api.Download("SPY", DateTime.Parse("2005/2/25"), DateTime.Now, "US", Stooq.Api.StooqApi.ETimeFrame.Weekly);

            Assert.IsNotNull(quotes, "Result object was not created");
            Assert.IsNotEmpty(quotes.Ticker, "Ticker field is not set");
            Assert.IsNotNull(quotes.Quotes, "Quotes array is not created");
            Assert.AreNotEqual(quotes.Quotes.Count, 0, "SPY quotes were not retrieved");
        }

        [Test]
        public void DownloadDaily_Success()
        {
            DMFX.Stooq.Api.StooqApi api = new Stooq.Api.StooqApi();

            DMFX.Stooq.Api.CSVQuotes quotes = api.Download("SPY", DateTime.Parse("2005/2/25"), DateTime.Now, "US", Stooq.Api.StooqApi.ETimeFrame.Daily);

            Assert.IsNotNull(quotes, "Result object was not created");
            Assert.IsNotEmpty(quotes.Ticker, "Ticker field is not set");
            Assert.IsNotNull(quotes.Quotes, "Quotes array is not created");
            Assert.AreNotEqual(quotes.Quotes.Count, 0, "SPY quotes were not retrieved");
        }

        [Test]
        public void DownloadMonthly_InvalidTicker()
        {
            try
            {
                DMFX.Stooq.Api.StooqApi api = new Stooq.Api.StooqApi();

                DMFX.Stooq.Api.CSVQuotes quotes = api.Download("#INVALID_TICKER#", DateTime.Parse("2005/2/25"), DateTime.Now, "US", Stooq.Api.StooqApi.ETimeFrame.Monthly);

                Assert.Fail("Failed to handle invalid ticker");
            }
            catch (Exception ex)
            {
                Assert.IsNotEmpty(ex.Message);
            }
            
        }
    }
}
