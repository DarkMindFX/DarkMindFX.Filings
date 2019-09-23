using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.SECParsers
{
    [TestFixture]
    public class TestAMZN
    {
        [Test]
        public void AMZN_10Q_Success_2017Q2()
        {
            DMFX.SECParser.SEC10Q.SEC10QDefaultParser parser = new DMFX.SECParser.SEC10Q.SEC10QDefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            secParams.FileContent = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AMZN_10-Q_2017Q2"]), FileMode.Open);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AMZN 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 5, string.Format("AMZN 10-Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[0].Title, result.Statements[0].Records.Count));
            Assert.GreaterOrEqual(result.Statements[1].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[1].Title, result.Statements[1].Records.Count));
            Assert.GreaterOrEqual(result.Statements[2].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[2].Title, result.Statements[2].Records.Count));
            Assert.GreaterOrEqual(result.Statements[3].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[3].Title, result.Statements[3].Records.Count));
            Assert.GreaterOrEqual(result.Statements[4].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[4].Title, result.Statements[4].Records.Count));

        }

        [Test]
        public void AMZN_10Q_Success_2017Q1()
        {
            DMFX.SECParser.SEC10Q.SEC10QDefaultParser parser = new DMFX.SECParser.SEC10Q.SEC10QDefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AMZN_10-Q_2017Q1"]);
            secParams.FileContent = new FileStream(path, FileMode.Open);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AMZN 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 5, string.Format("AMZN 10-Q: Invalid number of segments returned, Expected - 10, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[0].Title, result.Statements[0].Records.Count));
            Assert.GreaterOrEqual(result.Statements[1].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[1].Title, result.Statements[1].Records.Count));
            Assert.GreaterOrEqual(result.Statements[2].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[2].Title, result.Statements[2].Records.Count));
            Assert.GreaterOrEqual(result.Statements[3].Records.Count, 10, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[3].Title, result.Statements[3].Records.Count));
            Assert.GreaterOrEqual(result.Statements[4].Records.Count, 8, string.Format("AMZN 10Q: Invalid number of records in statement '{0}' returned", result.Statements[4].Title, result.Statements[4].Records.Count));

        }
    }
}
