// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using DMFX.CFTC.Api;
using System.Linq;

namespace DMFX.Test.CFTCApi
{
    [TestFixture]
    public class TestCFTCApiCalls
    {
        [Test]
        public void ImportRecent_COTFinFutOpt_Recent_Success()
        {
            ICFTCParserParams parserParams = new CFTCParserParamsCOTFinFutOpt();
            parserParams.OnlyLast = true;

            CFTCParser parser = new CFTCParser();

            ICFTCParserResult result = parser.Parse(parserParams);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Instruments);
            Assert.NotNull(result.Instruments.FirstOrDefault());
            Assert.IsNotEmpty(result.Instruments.FirstOrDefault().Value.Quotes);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Ticker);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Description);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Ticker, string.Empty);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Description, string.Empty);
        }

        [Test]
        public void ImportRecent_COTFinFutOpt_Historical_Success()
        {
            ICFTCParserParams parserParams = new CFTCParserParamsCOTFinFutOpt();
            parserParams.OnlyLast = false;

            CFTCParser parser = new CFTCParser();

            ICFTCParserResult result = parser.Parse(parserParams);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Instruments);
            Assert.NotNull(result.Instruments.FirstOrDefault());
            Assert.IsNotEmpty(result.Instruments.FirstOrDefault().Value.Quotes);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Ticker);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Description);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Ticker, string.Empty);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Description, string.Empty);
        }

        [Test]
        public void ImportRecent_COTCmdtsFutOpt_Recent_Success()
        {
            ICFTCParserParams parserParams = new CFTCParserParamsCOTCmdtsFutOpt();
            parserParams.OnlyLast = true;

            CFTCParser parser = new CFTCParser();

            ICFTCParserResult result = parser.Parse(parserParams);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Instruments);
            Assert.NotNull(result.Instruments.FirstOrDefault());
            Assert.IsNotEmpty(result.Instruments.FirstOrDefault().Value.Quotes);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Ticker);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Description);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Ticker, string.Empty);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Description, string.Empty);
        }

        [Test]
        public void ImportRecent_COTCmdtsFutOpt_Historical_Success()
        {
            ICFTCParserParams parserParams = new CFTCParserParamsCOTCmdtsFutOpt();
            parserParams.OnlyLast = false;

            CFTCParser parser = new CFTCParser();

            ICFTCParserResult result = parser.Parse(parserParams);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Instruments);
            Assert.NotNull(result.Instruments.FirstOrDefault());
            Assert.IsNotEmpty(result.Instruments.FirstOrDefault().Value.Quotes);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Ticker);
            Assert.NotNull(result.Instruments.FirstOrDefault().Value.Description);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Ticker, string.Empty);
            Assert.AreNotEqual(result.Instruments.FirstOrDefault().Value.Description, string.Empty);
        }
    }
}
