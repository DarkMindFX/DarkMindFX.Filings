using System;
using NUnit.Framework;

namespace DMFX.Text.YahooFinanceApi
{
    [TestFixture]
    public class TestYahooFinanceApiCalls
    {
        [Test]
        public void DownloadMonthly_Success()
        {
            DMFX.YahooFinance.Api.YahooFinanceApi api = new DMFX.YahooFinance.Api.YahooFinanceApi();

            DMFX.YahooFinance.Api.CSVQuotes quotes = api.Download("SPY", DateTime.Parse("2005/2/25"), DateTime.Now, YahooFinance.Api.YahooFinanceApi.ETimeFrame.Monthly);

            Assert.IsNotNull(quotes, "Result object was not created");
            Assert.IsNotEmpty(quotes.Ticker, "Ticker field is not set");
            Assert.IsNotNull(quotes.Quotes, "Quotes array is not created");
            Assert.AreNotEqual(quotes.Quotes.Count, 0, "SPY quotes were not retrieved");
        }

        [Test]
        public void DownloadWeekly_Success()
        {
            DMFX.YahooFinance.Api.YahooFinanceApi api = new YahooFinance.Api.YahooFinanceApi();

            DMFX.YahooFinance.Api.CSVQuotes quotes = api.Download("SPY", DateTime.Parse("2005/2/25"), DateTime.Now, YahooFinance.Api.YahooFinanceApi.ETimeFrame.Weekly);

            Assert.IsNotNull(quotes, "Result object was not created");
            Assert.IsNotEmpty(quotes.Ticker, "Ticker field is not set");
            Assert.IsNotNull(quotes.Quotes, "Quotes array is not created");
            Assert.AreNotEqual(quotes.Quotes.Count, 0, "SPY quotes were not retrieved");
        }

        [Test]
        public void DownloadDaily_Success()
        {
            DMFX.YahooFinance.Api.YahooFinanceApi api = new YahooFinance.Api.YahooFinanceApi();

            DMFX.YahooFinance.Api.CSVQuotes quotes = api.Download("SPY", DateTime.Parse("2005/2/25"), DateTime.Now, YahooFinance.Api.YahooFinanceApi.ETimeFrame.Daily);

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
                DMFX.YahooFinance.Api.YahooFinanceApi api = new YahooFinance.Api.YahooFinanceApi();

                DMFX.YahooFinance.Api.CSVQuotes quotes = api.Download("#INVALID_TICKER#", DateTime.Parse("2005/2/25"), DateTime.Now, YahooFinance.Api.YahooFinanceApi.ETimeFrame.Monthly);

                Assert.Fail("Failed to handle invalid ticker");
            }
            catch (Exception ex)
            {
                Assert.IsNotEmpty(ex.Message);
            }

        }
    }
}
