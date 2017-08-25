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
    public class TestTSLA
    {
        [Test]
        public void Parse_10Q_Success()
        {
            DMFX.SECParser.SEC10Q.TSLA parser = new DMFX.SECParser.SEC10Q.TSLA();

            // parser
            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            secParams.FileContent = new FileStream( Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\..\\..\\", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_TSLA_10-Q_2017Q2"]), FileMode.Open);

            // running parser
            var result = parser.Parse(secParams);
        }
    }
}
