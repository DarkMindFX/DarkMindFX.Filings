using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.SECParser.SECForm13F;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace DMFX.Test.SECParsers
{
    [TestFixture]
    public class TestBLK
    {
        [Test]
        public void BLK_FORM13F_09302014()
        {
            var parser = PrepareParser();

            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            Stream sPrimary = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_BLK_FORM13F_09302014_Primary"]), FileMode.Open);
            Stream sTable = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_BLK_FORM13F_09302014_Table"]), FileMode.Open);
            secParams.FileContent.Add("primary_doc.xml", sPrimary);
            secParams.FileContent.Add("form13fInfoTable.xml", sTable);

            // running parser
            var result = parser.Parse(secParams);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void BLK_FORM13F_09302017()
        {
            var parser = PrepareParser();

            SECParser.SECParserParams secParams = new SECParser.SECParserParams();

            // parameters
            Stream sPrimary = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_BLK_FORM13F_09302017_Primary"]), FileMode.Open);
            Stream sTable = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Sample Reportings", ConfigurationManager.AppSettings["File_SEC_BLK_FORM13F_09302017_Table"]), FileMode.Open);
            secParams.FileContent.Add("primary_doc.xml", sPrimary);
            secParams.FileContent.Add("form13fInfoTable.xml", sTable);

            // running parser
            var result = parser.Parse(secParams);

            Assert.IsTrue(result.Success);
        }

        private SECForm13FDefaultParser PrepareParser()
        {
            var parser = new SECForm13FDefaultParser();        


            return parser;
        }
    }
}
