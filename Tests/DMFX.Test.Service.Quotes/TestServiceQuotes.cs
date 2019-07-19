using DMFX.Service.DTO;
using DMFX.Test.Service.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Service.Quotes
{
    [TestFixture]
    public class TestServiceQuotes : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            string serviceUrl = ConfigurationManager.AppSettings["QuotesServiceURL"];

            Dictionary<string, SqlConnection> connections = new Dictionary<string, SqlConnection>();
            
            Init(serviceUrl, connections, null);

            this.SessionToken = ConfigurationManager.AppSettings["QuotesServiceSessionToken"];
        }

        [TearDown]
        public void TerDown()
        {
        }

        [TestCase("000.GetQuotes.Success")]
        public void TestGetQuotesSuccess(string name)
        {
            GetQuotes request = PrepareRequest<GetQuotes>(name);

            GetQuotesResponse response = Post<GetQuotes, GetQuotesResponse>("GetQuotes", request);

            Assert.IsTrue(response.Success, "Failed to get quotes");
        }

        [TestCase("001.GetQuotes.InvalidCountryCode")]
        [TestCase("002.GetQuotes.InvalidTicker")]
        public void TestGetQuotesFail(string name)
        {
            GetQuotes request = PrepareRequest<GetQuotes>(name);

            GetQuotesResponse response = Post<GetQuotes, GetQuotesResponse>("GetQuotes", request);

            Assert.IsFalse(response.Success, "GetQuotes succeeded unexpectedly");
        }
    }
}
