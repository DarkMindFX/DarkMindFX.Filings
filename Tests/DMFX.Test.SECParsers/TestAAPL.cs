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
            Stream s = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q2"]), FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q2"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - 60, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));
            

        }

        [Test]
        public void AAPL_10Q_Success_2017Q1()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q1"]);
            Stream s = new FileStream(path, FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q1"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - 60, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));
            
        }

        [Test]
        public void AAPL_10Q_Success_2016Q4()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q4"]);
            Stream s = new FileStream(path, FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q4"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned, Expected - 60, Actual - {1}", result.Statements[0].Title, result.Statements[0].Records.Count));
            
        }

        [Test]
        public void AAPL_10Q_Success_2016Q2()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q2"]);
            var s = new FileStream(path, FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q2"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned", result.Statements[0].Title));
            
        }

        [Test]
        public void AAPL_10Q_Success_2016Q1()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q1"]);
            var s = new FileStream(path, FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q1"], s) ;

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 5, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned", result.Statements[0].Title));
            
        }

        [Test]
        public void AAPL_10Q_Success_2011Q3()
        {
            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2011Q3"]);
            var s = new FileStream(path, FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2011Q3"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL 10-Q: Parse failed with {0} errors", result.Errors.Count.ToString());

            // checking number of segments
            Assert.AreEqual(result.Statements.Count, 1, string.Format("AAPL 10Q: Invalid number of segments returned, Expected - 1, Actual - {0}", result.Statements.Count));

            // checking records count per segment
            Assert.GreaterOrEqual(result.Statements[0].Records.Count, 10, string.Format("AAPL 10Q: Invalid number of records in statement '{0}' returned", result.Statements[0].Title));
            
        }

        [Test]
        public void AAPL_10Q_Success_ConseqReports()
        {
            string[] reports = new string[]
            {
                ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q2"],
                ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2017Q1"],
                ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q4"],
                ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q2"],
                ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2016Q1"],
                ConfigurationManager.AppSettings["File_SEC_AAPL_10-Q_2015Q4"]
            };

            DMFX.SECParser.SEC10Q.AAPL parser = new DMFX.SECParser.SEC10Q.AAPL();

            for (int i = 0; i < reports.Count(); ++i)
            {

                SECParser.SECParserParams secParams = new SECParser.SECParserParams();

                // parameters
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", reports[i]);
                var s = new FileStream(path, FileMode.Open);
                secParams.FileContent.Add(reports[i], s);

                // running parser
                var result = parser.Parse(secParams);

                for (int a = 0; a < result.Statements.Count; ++a)
                {
                    Assert.IsNotEmpty(result.Statements[a].Records, string.Format("Statement {0} is empty - run {1}", result.Statements[a].Title, i));
                }
            }
        }

        [Test]
        public void AAPL_Form4_Success_20190502()
        {
            DMFX.SECParser.SECForm4.SECForm4DefaultParser parser = new DMFX.SECParser.SECForm4.SECForm4DefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            var s = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_FORM4_20190502"]), FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_FORM4_20190502"], s); 

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL Form 4 2019/05/02: Parse failed with {0} errors", result.Errors.Count.ToString());

            
        }

        [Test]
        public void AAPL_Form4_Success_20190812()
        {
            DMFX.SECParser.SECForm4.SECForm4DefaultParser parser = new DMFX.SECParser.SECForm4.SECForm4DefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            var s = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_FORM4_20190812"]), FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_FORM4_20190812"], s) ;

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL Form 4 2019/08/12: Parse failed with {0} errors", result.Errors.Count.ToString());
        }

        [Test]
        public void AAPL_Form4_Success_20190824()
        {
            DMFX.SECParser.SECForm4.SECForm4DefaultParser parser = new DMFX.SECParser.SECForm4.SECForm4DefaultParser();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            var s = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_AAPL_FORM4_20190824"]), FileMode.Open);
            secParams.FileContent.Add(ConfigurationManager.AppSettings["File_SEC_AAPL_FORM4_20190824"], s);

            // running parser
            var result = parser.Parse(secParams);

            // checking error code
            Assert.AreEqual(result.Success, true, "AAPL Form 4 2019/08/24: Parse failed with {0} errors", result.Errors.Count.ToString());
        }
    }
}
