using DMFX.Interfaces;
using DMFX.SEC.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.Source.SEC
{

    [Export("SEC", typeof(ISource))]
    public class SECSource : ISource
    {
        SECApi _secApi = null;
        IDictionary _dictionary = null;

        [ImportingConstructor]
        public SECSource([Import("File")]IDictionary dict)
        {
            _secApi = new SECApi();
            _dictionary = dict;
        }
        public ISourceExtractResult ExtractReports(ISourceExtractParams extractParams)
        {
            SECSourceExtractResult result = new SECSourceExtractResult();

            SECSourceExtractParams extractSECParams = extractParams as SECSourceExtractParams;
            if (extractSECParams != null)
            {
                string cik = _dictionary.LookupRegulatorCompanyCode(extractSECParams.RegulatorCode, extractSECParams.CompanyCode); // TODO: lookup in dictionary
                if (!string.IsNullOrEmpty(cik))
                {
                    foreach (var filing in extractSECParams.Items)
                    {
                        // getting list of files for each filing
                        Submission submission = _secApi.ArchivesEdgarDataCIKSubmission(cik, filing.Name);
                        if (submission != null)
                        {
                            foreach (var fileInfo in submission.Files)
                            {
                                // to speed up we need to extract only xml files and index headers file
                                if (Path.GetExtension(fileInfo.Name) == ".xml" || fileInfo.Name.Contains(".txt"))
                                {
                                    SubmissionFile file = _secApi.ArchivesEdgarDataCIKSubmissionFile(cik, filing.Name, fileInfo.Name);
                                    if (file != null)
                                    {
                                        SECSourceItem sourceItem = new SECSourceItem();
                                        sourceItem.Name = fileInfo.Name;
                                        sourceItem.FilingName = filing.Name;
                                        sourceItem.CompanyCode = extractSECParams.CompanyCode;
                                        sourceItem.RegulatorCode = extractSECParams.RegulatorCode;
                                        sourceItem.Content = file.Content;

                                        result.Items.Add(sourceItem);
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.AddError(EErrorCodes.ImporterError, EErrorType.Warning, string.Format("Failed to import filing {0}", filing.Name));
                        }
                    }
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.AddError(EErrorCodes.InvalidParserParams, EErrorType.Error, string.Format("Cannot find the SEC CIK for company with code {0}", extractSECParams.CompanyCode));
                }
            }
            else
            {
                result.Success = false;
                result.AddError(EErrorCodes.InvalidParserParams, EErrorType.Error, "Parameters of invalid type were provided");
            }

            return result;
        }

        public ISourceValidateResult ValidateSourceDelta(ISourceValidateParams vldParams)
        {
            SECSourceValidateResult result = new SECSourceValidateResult();

            SECSourceValidateParams vldSECParams = vldParams as SECSourceValidateParams;
            if (vldSECParams != null)
            {
                string cik = _dictionary.LookupRegulatorCompanyCode(vldParams.RegulatorCode, vldParams.CompanyCode); // TODO: lookup in dictionary
                if (!string.IsNullOrEmpty(cik))
                {

                    Submissions submissions = _secApi.ArchivesEdgarDataCIK(cik);

                    // TODO: here we need to check what is the last filing in our DB - for now just returning last 3 years
                    DateTime lastUpdated = vldSECParams.UpdateFrom;

                    result.NeedUpdate = true; // TODO: this value depends on whether there are any new filings for the company - for now always true
                    foreach (var filing in submissions.Folders.OrderByDescending(x => x.LastModified).Where(x => x.LastModified > lastUpdated))
                    {
                        SECSourceItemInfo secSourceItemInfo = new SECSourceItemInfo();
                        secSourceItemInfo.Name = filing.Name;
                        result.Delta.Add(secSourceItemInfo);
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.AddError(EErrorCodes.InvalidParserParams, EErrorType.Error, string.Format("Cannot find the SEC CIK for company with code {0}", vldParams.CompanyCode));
                }

            }

            return result;
        }


        public ISourceSubmissionsInfoResult GetSubmissionsInfo(ISourceSubmissionsInfoParams infoParams)
        {
            ISourceSubmissionsInfoResult result = new SECSourceSubmissionsInfoResult();

            SECSourceSubmissionsInfoParams secInfoParams = infoParams as SECSourceSubmissionsInfoParams;
            if (secInfoParams != null)
            {
                string cik = _dictionary.LookupRegulatorCompanyCode(infoParams.RegulatorCode, infoParams.CompanyCode); // TODO: lookup in dictionary
                // for each submission - extracting content and checking type
                int count = 0;
                foreach (var item in secInfoParams.Items)
                {
                    Submission submission = _secApi.ArchivesEdgarDataCIKSubmission(cik, item.Name);
                    if (submission != null)
                    {
                        try
                        {

                            // extracting txt index file
                            SubmissionFileInfo subFileInfo = submission.Files.FirstOrDefault(s => s.Name.Contains(".txt"));
                            SubmissionFile indexFile = null;

                            if (subFileInfo != null)
                            {
                                indexFile = _secApi.ArchivesEdgarDataCIKSubmissionFile(cik, item.Name, subFileInfo.Name);
                            }

                            if (indexFile != null)
                            {
                                SECSourceSubmissionInfo submissionInfo = ExtractReportDetails(indexFile);
                                if (submissionInfo != null)
                                {
                                    submissionInfo.Name = item.Name;
                                    result.Submissions.Add(submissionInfo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.AddError(new Error() { Code = EErrorCodes.ImporterError, Message = string.Format("Report'{0}', Error: {1}", item.Name, ex.Message) });
                        }
                    }
                    else
                    {
                        result.Errors.Add(new Error() { Code = EErrorCodes.SubmissionNotFound, Type = EErrorType.Warning, Message = string.Format("Submission '{0}' was not found", item.Name) });
                    }

                    ++count;

                }

                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.Errors.Add(new Error() { Code = EErrorCodes.InvalidSourceParams, Type = EErrorType.Error, Message = "Invalid parameter provided" });
            }

            return result;
        }
                

        public ISourceExtractResult ExtractFilingItems(ISourceExtractFilingItemsParams extractItemsParams)
        {
            SECSourceExtractResult result = new SECSourceExtractResult();
            SECSourceExtractFilingItemsParams extractSECItemsParams = extractItemsParams as SECSourceExtractFilingItemsParams;
            if (extractSECItemsParams != null)
            {
                string cik = _dictionary.LookupRegulatorCompanyCode(extractSECItemsParams.RegulatorCode, extractSECItemsParams.CompanyCode); // TODO: lookup in dictionary
                if (!string.IsNullOrEmpty(cik))
                {
                    foreach (var item in extractSECItemsParams.Items)
                    {
                        SubmissionFile file = _secApi.ArchivesEdgarDataCIKSubmissionFile(cik, extractSECItemsParams.Filing.Name, item.Name);
                        if (file != null)
                        {
                            SECSourceItem sourceItem = new SECSourceItem();
                            sourceItem.Name = item.Name;
                            sourceItem.FilingName = extractSECItemsParams.Filing.Name;
                            sourceItem.CompanyCode = extractItemsParams.CompanyCode;
                            sourceItem.RegulatorCode = extractItemsParams.RegulatorCode;
                            sourceItem.Content = file.Content;

                            result.Items.Add(sourceItem);
                        }

                    }
                }
            }

            return result;
        }

        public ISourceExtractFilingItemsParams CreateSourceExtractFilingItemsParams()
        {
            return new SECSourceExtractFilingItemsParams();
        }

        public ISourceValidateParams CreateValidateParams()
        {
            return new SECSourceValidateParams();
        }

        public ISourceExtractParams CreateExtractParams()
        {
            return new SECSourceExtractParams();
        }

        public ISourceItemInfo CreateSourceItemInfo()
        {
            return new SECSourceItemInfo();
        }

        public ISourceSubmissionsInfoParams CreateSourceSubmissionsInfoParams()
        {
            return new SECSourceSubmissionsInfoParams();
        }

        #region Support  methods
        private SECSourceSubmissionInfo ExtractReportDetails(SubmissionFile submissionIndexFile)
        {
            SECSourceSubmissionInfo subInfo = new SECSourceSubmissionInfo();

            string txtContent = System.Text.Encoding.Default.GetString(submissionIndexFile.Content.ToArray());

            // fixing tag brackets
            txtContent = txtContent.Replace("&lt;", "<").Replace("&gt;", ">");

            string[] txtContentLines = txtContent.Split(new char[] { '\n' });

            string strType = GetValue(txtContentLines, "CONFORMED SUBMISSION TYPE");
            string strReportingPeriod = GetValue(txtContentLines, "CONFORMED PERIOD OF REPORT");
            string strDateFiled = GetValue(txtContentLines, "FILED AS OF DATE");

            subInfo.Type = strType;
            subInfo.PeriodEnd = !string.IsNullOrEmpty(strReportingPeriod) ? ParseTime(strReportingPeriod) : DateTime.MinValue;
            subInfo.Submitted = !string.IsNullOrEmpty(strDateFiled) ? ParseTime(strDateFiled) : DateTime.MinValue;

            if (subInfo.Type == "10-Q")
            {
                subInfo.Report = Extract10QReportFilename(txtContentLines);
            }
            /*
            else if (subInfo.Type == "10-K")
            {
                subInfo.Report = Extract10KReportFilename(txtContentLines);
            }
            */
            else
            {
                subInfo = null;
            }

            return subInfo;
        }

        private string Extract10QReportFilename(string[] txtContentLines)
        {
            string result = null;

            int currLine = 0;
            bool typeFound = false;

            while (currLine < txtContentLines.Count() && result == null)
            {
                string strCurrent = txtContentLines[currLine];

                if(strCurrent.Contains("<TYPE>EX-101.INS"))
                {
                    typeFound = true;
                }
                if (strCurrent.Contains("<FILENAME>") && typeFound)
                {
                    result = strCurrent.Replace("<FILENAME>", string.Empty);
                }                

                ++currLine;
            }

            return result;
        }

        private string Extract10KReportFilename(string[] txtContentLines)
        {
            string result = null;

            return result;
        }

        private DateTime ParseTime(string strDate)
        {
            DateTime result = DateTime.MinValue;

            int year = Int32.Parse(strDate.Substring(0, 4));
            int month = Int32.Parse(strDate.Substring(4, 2));
            int day = Int32.Parse(strDate.Substring(6, 2));

            result = new DateTime(year, month, day);

            return result;
        }

        private string GetValue(string[] txtContentLines, string name)
        {
            string result = string.Empty;
            string strType = txtContentLines.FirstOrDefault(s => s.Contains(name));
            if (!string.IsNullOrEmpty(strType))
            {
                string[] typeParts = strType.Trim().Split(new char[] { ':' });
                result = typeParts[1].Trim();
            }

            return result;
        }
        #endregion
    }
}
