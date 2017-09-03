using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Reflection;
using DMFX.Interfaces;

namespace DMFX.Test.SECParsers
{
    [TestFixture]
    public class TestAAPL
    {
        [Test]
        public void AAPL_10Q_Success_2017Q2()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            secParams.FileContent = new FileStream( Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..","..","..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q2"]), FileMode.Open);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 5, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 60, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - 60, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));
            Assert.GreaterOrEqual(result.Statements[1].Records.Count, 40, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 40, Actual - {1}", result.Statements[1].Title, result.Statements[1].Records.Count));
            Assert.GreaterOrEqual(result.Statements[2].Records.Count, 58, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 58, Actual - {1}", result.Statements[2].Title, result.Statements[2].Records.Count));
            Assert.GreaterOrEqual(result.Statements[3].Records.Count, 68, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 68, Actual - {1}", result.Statements[3].Title, result.Statements[3].Records.Count));
            Assert.GreaterOrEqual(result.Statements[4].Records.Count, 20, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 20, Actual - {1}", result.Statements[4].Title, result.Statements[4].Records.Count));
            
        }

        [Test]
        public void AAPL_10Q_Success_2017Q1()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q1"]);
            secParams.FileContent = new FileStream(path, FileMode.Open);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 5, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 60, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - 60, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));
            Assert.GreaterOrEqual(result.Statements[1].Records.Count, 40, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 40, Actual - {1}", result.Statements[1].Title, result.Statements[1].Records.Count));
            Assert.GreaterOrEqual(result.Statements[2].Records.Count, 58, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 58, Actual - {1}", result.Statements[2].Title, result.Statements[2].Records.Count));
            Assert.GreaterOrEqual(result.Statements[3].Records.Count, 68, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 68, Actual - {1}", result.Statements[3].Title, result.Statements[3].Records.Count));
            Assert.GreaterOrEqual(result.Statements[4].Records.Count, 20, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - greater 20, Actual - {1}", result.Statements[4].Title, result.Statements[4].Records.Count));
        }
    }
}
