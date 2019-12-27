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
    public class TestBAC
    {
        [Test]
        public void BAC_10Q_Success_2017Q2()
        {
            DMFX.SECParser.SEC10Q.SEC10QDefaultParser parser = new DMFX.SECParser.SEC10Q.SEC10QDefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            var s = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_BAC_10-Q_2017Q2"]), FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_BAC_10-Q_2017Q2"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "BAC 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("BAC 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 600, string.Format("BAC 10Q: Invalid number of records in statement '{0}' returned, Expected - 600, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));            

        }

        [Test]
        public void BAC_10Q_Success_2017Q1()
        {
            DMFX.SECParser.SEC10Q.SEC10QDefaultParser parser = new DMFX.SECParser.SEC10Q.SEC10QDefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_BAC_10-Q_2017Q1"]);
            var s = new FileStream(path, FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_BAC_10-Q_2017Q1"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "BAC 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("BAC 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("BAC 10Q: Invalid number of records in statement '{0}' returned, Expected - 60, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));
            
        }
    }
}
