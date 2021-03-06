﻿using DMFX.Dictionaries;
using DMFX.Interfaces;
using DMFX.Source.SEC;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Sources
{
    [TestFixture]
    public class TestSECSource
    {
        [Test]
        public void SourceAAPL_Validate_Success()
        {
            FileDictionary fileDict = new FileDictionary();
            ISource secSource = new SECSource();
            ISourceInitParams initParams = secSource.CreateInitParams();
            initParams.Dictionary = fileDict;
            secSource.Init(initParams);

            // preparing params
            ISourceValidateParams vldParams = secSource.CreateValidateParams();
            vldParams.CompanyCode = ConfigurationManager.AppSettings["SEC_AAPL_CODE"];
            vldParams.RegulatorCode = ConfigurationManager.AppSettings["SECCode"];
            vldParams.UpdateFromDate = DateTime.Now - TimeSpan.FromDays(90);
            vldParams.UpdateToDate = DateTime.Now;

            ISourceFilingsListResult result = secSource.GetFilingsList(vldParams);

            Assert.AreNotEqual(result.Filings, null, "List of delta items is NULL");
            Assert.AreNotEqual(result.Filings.Count, 0, "List of delta items is empty");
        }

        [Test]
        public void SourceAAPL_Extract_Success()
        {
            FileDictionary fileDict = new FileDictionary();
            ISource secSource = new SECSource();
            ISourceInitParams initParams = secSource.CreateInitParams();
            initParams.Dictionary = fileDict;
            secSource.Init(initParams);

            ISourceExtractParams extractParams = secSource.CreateExtractParams();
            extractParams.CompanyCode = ConfigurationManager.AppSettings["SEC_AAPL_CODE"];
            extractParams.RegulatorCode = ConfigurationManager.AppSettings["SECCode"];

            ISourceItemInfo itemInfo = secSource.CreateSourceItemInfo();
            itemInfo.Name = ConfigurationManager.AppSettings["SEC_CIK_AAPL_SUBMISSION_FOLDER"];
            extractParams.Items.Add(itemInfo);

            ISourceExtractResult result = secSource.ExtractReports(extractParams);

            Assert.AreNotEqual(result.Items, null, "List of file items in filing is NULL");
            Assert.AreNotEqual(result.Items.Count, 0, "List of file items in filing is empty");
            Assert.AreEqual(result.Items.Count, 75, string.Format("Incorrect number of file extracted - {0}", result.Items.Count));

        }

        [Test]
        public void SourceAAPL_SubmissionInfo_Success()
        {
            FileDictionary fileDict = new FileDictionary();
            ISource secSource = new SECSource();
            ISourceInitParams initParams = secSource.CreateInitParams();
            initParams.Dictionary = fileDict;
            secSource.Init(initParams);

            ISourceSubmissionsInfoParams subInfoParams = new SECSourceSubmissionsInfoParams();
            subInfoParams.CompanyCode = ConfigurationManager.AppSettings["SEC_AAPL_CODE"];
            subInfoParams.RegulatorCode = ConfigurationManager.AppSettings["SECCode"];

            ISourceItemInfo itemInfo = secSource.CreateSourceItemInfo();
            itemInfo.Name = ConfigurationManager.AppSettings["SEC_CIK_AAPL_SUBMISSION_FOLDER"];
            subInfoParams.Items.Add(itemInfo);

            ISourceSubmissionsInfoResult subInfoResult = secSource.GetSubmissionsInfo(subInfoParams);

            Assert.IsTrue(subInfoResult.Success, "Failed to get submission info");
            Assert.AreNotEqual(subInfoResult.Submissions, null, "Submissions list is NULL");
            Assert.IsNotEmpty(subInfoResult.Submissions, "Submissions list is empty");
            Assert.AreEqual(subInfoResult.Submissions.Count, 1, "Invalid submissions info items in the list");
            Assert.AreEqual(subInfoResult.Submissions[0].Report, "aapl-20170701.xml", "Invalid report file specified");
            Assert.AreEqual(subInfoResult.Submissions[0].Name, ConfigurationManager.AppSettings["SEC_CIK_AAPL_SUBMISSION_FOLDER"], "Invalid submission name returned");
        }
    }
}
