using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.BLSApi
{
    [TestFixture]
    public class TestBLSApiCalls
    {
        [Test]
        public void DownloadCPI_Monthly_Success()
        {
            DMFX.BLS.Api.BLSApi api = new BLS.Api.BLSApi();

            string code = ConfigurationManager.AppSettings["CODE_CPI"];
            DateTime from = DateTime.Parse("2000/1/1");
            DateTime to = DateTime.Parse("2019/12/31");

            var result = api.Download(code, from, to);
            
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Ticker != null);
            Assert.IsTrue(result.Quotes != null);
            Assert.IsTrue(result.Quotes.Count > 0);
            Assert.IsTrue(result.Timeframe == BLS.Api.BLSApi.ETimeFrame.Monthly);

        }

        [Test]
        public void DownloadJobGains_Quarterly_Success()
        {
            DMFX.BLS.Api.BLSApi api = new BLS.Api.BLSApi();

            string code = ConfigurationManager.AppSettings["CODE_JOBGAINS"];
            DateTime from = DateTime.Parse("2000/1/1");
            DateTime to = DateTime.Parse("2019/12/31");

            var result = api.Download(code, from, to);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Ticker != null);
            Assert.IsTrue(result.Quotes != null);
            Assert.IsTrue(result.Quotes.Count > 0);
            Assert.IsTrue(result.Timeframe == BLS.Api.BLSApi.ETimeFrame.Quarterly);

        }

        [Test]
        public void DownloadLaborForcePartRate_Annual_Success()
        {
            DMFX.BLS.Api.BLSApi api = new BLS.Api.BLSApi();

            string code = ConfigurationManager.AppSettings["CODE_LABORFORCEPARTRATE"];
            DateTime from = DateTime.Parse("2000/1/1");
            DateTime to = DateTime.Parse("2019/12/31");

            var result = api.Download(code, from, to);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Ticker != null);
            Assert.IsTrue(result.Quotes != null);
            Assert.IsTrue(result.Quotes.Count > 0);
            Assert.IsTrue(result.Timeframe == BLS.Api.BLSApi.ETimeFrame.Annual);

        }
    }
}
