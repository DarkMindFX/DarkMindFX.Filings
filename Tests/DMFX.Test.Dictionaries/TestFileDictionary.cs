﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Dictionaries
{
    [TestFixture]
    public class TestFileDictionary
    {
        [Test]
        public void SECCompanyCIKByTicker_Success()
        {

            DMFX.Dictionaries.FileDictionary fileDictionary = new DMFX.Dictionaries.FileDictionary();
            string secCode = ConfigurationSettings.AppSettings["SECCode"];
            string ticker = ConfigurationSettings.AppSettings["SEC_AAPL_CODE"];
            string appleCIK = fileDictionary.LookupRegulatorCompanyCode(secCode, ticker);

            Assert.AreEqual(appleCIK, ConfigurationSettings.AppSettings["SEC_AAPL_CIK"], "Invalid AAPL CIK");
        }
    }
}
