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
    public class TestServiceTimeSeries : TestBase
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

        [TestCase("000.GetTimeSeries.Success")]
        public void TestGetQuotesSuccess(string name)
        {
            GetTimeSeries request = PrepareRequest<GetTimeSeries>(name);

            GetTimeSeriesResponse response = Post<GetTimeSeries, GetTimeSeriesResponse>("GetTimeSeries", request);

            Assert.IsTrue(response.Success, "Failed to get quotes");
        }

        //[TestCase("001.GetTimeSeries.InvalidCountryCode")] -- TODO - for now countries are not supported
        [TestCase("002.GetTimeSeries.InvalidTicker")]
        public void TestGetQuotesFail(string name)
        {
            GetTimeSeries request = PrepareRequest<GetTimeSeries>(name);

            GetTimeSeriesResponse response = Post<GetTimeSeries, GetTimeSeriesResponse>("GetTimeSeries", request);

            Assert.IsFalse(response.Success, "GetTimeSeries succeeded unexpectedly");
        }
    }
}
