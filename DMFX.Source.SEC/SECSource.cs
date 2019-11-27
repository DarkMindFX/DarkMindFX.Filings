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
using HtmlAgilityPack;


namespace DMFX.Source.SEC
{

    [Export("SEC", typeof(ISource))]
    public class SECSource : ISource
    {
        private SECApi _secApi = null;
        private IDictionary _dictionary = null;
        private IStorage _storage = null;
        private ILogger _logger = null;
        private bool _extractFromStorage = false;

        [ImportingConstructor]
        public SECSource()
        {
            _secApi = new SECApi();
            
        }

        public void Init(ISourceInitParams initParams)
        {
            _dictionary = initParams.Dictionary;
            _storage = initParams.Storage;
            _logger = initParams.Logger;
            _extractFromStorage = initParams.ExtractFromStorage;
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

                            // saving all uploaded items to storage
                            PutToStorage(result.Items);
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

        public ISourceFilingsListResult GetFilingsList(ISourceValidateParams vldParams)
        {
            SECSourceFilingsListResult result = new SECSourceFilingsListResult();

            SECSourceValidateParams vldSECParams = vldParams as SECSourceValidateParams;
            if (vldSECParams != null)
            {
                string cik = _dictionary.LookupRegulatorCompanyCode(vldParams.RegulatorCode, vldParams.CompanyCode);
                if (!string.IsNullOrEmpty(cik))
                {

                    Submissions submissions = _secApi.ArchivesEdgarDataCIK(cik);
                    
                    foreach (var filing in submissions.Folders.OrderBy( x => x.LastModified ).Where(x => x.LastModified >= vldSECParams.UpdateFromDate && x.LastModified <= vldSECParams.UpdateToDate))
                    {
                        SECSourceItemInfo secSourceItemInfo = new SECSourceItemInfo();
                        secSourceItemInfo.Name = filing.Name;
                        result.Filings.Add(secSourceItemInfo);
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
                    Submission submission = !_extractFromStorage ? 
                        GetSubmissionFromApi(cik, item.Name) : 
                        GetSubmissionFromStorage(infoParams.RegulatorCode, infoParams.CompanyCode, item.Name);

                    if (submission != null)
                    {
                        try
                        {
                            // extracting txt index file
                            SubmissionFileInfo subFileInfo = submission.Files.FirstOrDefault(s => s.Name.Contains("-index.html"));
                            SubmissionFile indexFile = null;

                            if (subFileInfo != null)
                            {
                                indexFile = !_extractFromStorage ?  
                                    _secApi.ArchivesEdgarDataCIKSubmissionFile(cik, item.Name, subFileInfo.Name) : 
                                    LoadFromStorage(infoParams.RegulatorCode, infoParams.CompanyCode, item.Name, subFileInfo.Name);
                            }

                            if (indexFile != null)
                            {
                                SECSourceSubmissionInfo submissionInfo = ExtractReportDetailsIndexHTML(indexFile);
                                if (submissionInfo != null && !string.IsNullOrEmpty(submissionInfo.Type))
                                {
                                    if (!_extractFromStorage)
                                    {
                                        PutToStorage(infoParams.RegulatorCode, infoParams.CompanyCode, submission.Name, indexFile);
                                    }                                   

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
                        SubmissionFile file = null;
                        if (!_extractFromStorage)
                        {
                            file = _secApi.ArchivesEdgarDataCIKSubmissionFile(cik, extractSECItemsParams.Filing.Name, item.Name);
                        }
                        else
                        {
                            file = LoadFromStorage(extractSECItemsParams.RegulatorCode, extractSECItemsParams.CompanyCode, extractSECItemsParams.Filing.Name, item.Name);
                        }
                        if (file != null)
                        {
                            SECSourceItem sourceItem = ToSourceItem(extractItemsParams.RegulatorCode, extractItemsParams.CompanyCode, extractSECItemsParams.Filing.Name, file);
                            
                            result.Items.Add(sourceItem);
                        }
                    }

                    if (!_extractFromStorage)
                    {
                        PutToStorage(result.Items);
                    }
                }
            }

            return result;
        }

        #region Create params methods

        public ISourceInitParams CreateInitParams()
        {
            return new SECSourceinitParams();
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

        #endregion

        #region Support  methods

        private Submission GetSubmissionFromApi(string cik, string name)
        {
            Submission result = _secApi.ArchivesEdgarDataCIKSubmission(cik, name);

            return result;
        }

        private Submission GetSubmissionFromStorage(string regualtorCode, string companyCode, string name)
        {
            Submission result = null;

            if (_storage != null)
            {
                List<string> items = new List<string>();
                if (_storage.ListItems(regualtorCode, companyCode, name, items) == EErrorCodes.Success)
                {
                    result = new Submission(name, DateTime.UtcNow);
                    foreach (var item in items)
                    {
                        result.Files.Add(new SubmissionFileInfo(Path.GetFileName(item), DateTime.UtcNow));
                    }
                }
            }

            return result;
        }

        private Submission GetSubmission<SECApi>(string cik, string name)
        {
            Submission result = null;

            return result;
        }

        private SubmissionFile LoadFromStorage(string regulatorCode, string companyCode, string filingName, string name)
        {
            SubmissionFile file = null;
            if (_storage != null)
            {
                List<byte> content = new List<byte>();
                EErrorCodes loadRes = _storage.Load(regulatorCode, companyCode, filingName, name, content);
                if (loadRes == EErrorCodes.Success)
                {
                    file = new SubmissionFile(name);
                    file.Content = content;
                }
            }

            return file;
        }

        private SECSourceItem ToSourceItem(string regulatorCode, string companyCode, string filingName, SubmissionFile file)
        {
            SECSourceItem result = new SECSourceItem();
            result.RegulatorCode = regulatorCode;
            result.CompanyCode = companyCode;
            result.FilingName = filingName;
            result.Name = file.Name;
            result.Content = file.Content;

            return result;
        }

        private void PutToStorage(string regulatorCode, string companyCode, string filingName, SubmissionFile file)
        {
            List<ISourceItem> items = new List<ISourceItem>();
            items.Add(ToSourceItem(regulatorCode, companyCode, filingName, file));
            PutToStorage(items);
        }

        private void PutToStorage(List<ISourceItem> items)
        {
            if (_storage != null)
            {
                foreach (var item in items)
                {
                    try
                    { 
                        _storage.Save(item.RegulatorCode, item.CompanyCode, item.FilingName, item.Name, item.Content);
                        if (_logger != null)
                        {
                            _logger.Log(EErrorType.Info, string.Format("Item saved: {0}/{1}/{2}/{3}", item.RegulatorCode, item.CompanyCode, item.FilingName, item.Name));
                        }
                    }
                    catch (Exception)
                    {
                        if (_logger != null)
                        {
                            _logger.Log(EErrorType.Error, string.Format("Save failed: {0}/{1}/{2}/{3}", item.RegulatorCode, item.CompanyCode, item.FilingName, item.Name));
                        }
                    }
                }
            }
        }

        #region IndexHTML

        private SECSourceSubmissionInfo ExtractReportDetailsIndexHTML(SubmissionFile submissionIndexFile)
        {
            SECSourceSubmissionInfo subInfo = new SECSourceSubmissionInfo();


            string txtContent = System.Text.Encoding.Default.GetString(submissionIndexFile.Content.ToArray());

            var doc = new HtmlDocument();
            doc.LoadHtml(txtContent);


            HtmlNode nodeType = doc.DocumentNode.SelectSingleNode("//div[@id='formDiv']/div[@id='formHeader']/div[@id='formName']/strong"); // 
            if (nodeType != null)
            {
                if (nodeType.InnerText.Contains("10-Q"))
                {
                    subInfo.Type = "10-Q";
                }
                
                else if (nodeType.InnerText.Contains("10-K"))
                {
                    subInfo.Type = "10-K";
                }

                else if(nodeType.InnerText.Contains("Form 424B2"))
                {
                    subInfo.Type = "424B2";
                }

                else if (nodeType.InnerText.Contains("Form 4"))
                {
                    subInfo.Type = "4";
                }

                if (!string.IsNullOrEmpty(subInfo.Type))
                {
                    // extracting dates
                    var nodesMetadata = doc.DocumentNode.SelectNodes("//div[@id='formDiv']/div/div/div[@class='infoHead']");
                    if (nodesMetadata != null)
                    {
                        foreach (HtmlNode node in nodesMetadata)
                        {
                            HtmlNode nodeDate = node.SelectSingleNode("../div[@class='info']");
                            if (nodeDate != null)
                            {
                                if (node.InnerText == "Accepted")
                                {
                                    subInfo.Submitted = DateTime.Parse(nodeDate.InnerText);
                                }
                                else if (node.InnerText == "Period of Report")
                                {
                                    subInfo.PeriodEnd = DateTime.Parse(nodeDate.InnerText);
                                }
                            }
                        }
                    }
                    // extracting report file name
                    HtmlNode nodeFilingData = doc.DocumentNode.SelectSingleNode("//div[@id='formDiv']/div/table/tr/td[text()='EX-101.INS']/..");
                    if (nodeFilingData == null)
                    {
                        switch (subInfo.Type)
                        {
                            case "10-Q":
                            case "10-K":
                                nodeFilingData = doc.DocumentNode.SelectSingleNode("//div[@id='formDiv']/div/table/tr/td[text()='XML']/..");
                                break;
                            case "4":
                                {
                                    var fileNodes = doc.DocumentNode.SelectNodes("//div[@id='formDiv']/div/table/tr/td[text()='4']/..");
                                    nodeFilingData = fileNodes.FirstOrDefault(x => x.SelectSingleNode("td/a") != null && x.SelectSingleNode("td/a").InnerText.IndexOf(".xml") >= 0);
                                }
                                break;
                        }
                    }
                    if (nodeFilingData != null)
                    {
                        HtmlNode nodeFileName = nodeFilingData.SelectSingleNode("td/a");
                        subInfo.Report = nodeFileName.InnerText.Trim();
                    }
                }
            }

            return subInfo;
        }

        private string FixIndexXmlContent(string content)
        {
            string result = content;
            // index.html file usually lacks closing tags - fixing it
            if (!result.Contains("</body>"))
            {
                result += "</body>";
            }
            if (!result.Contains("</html>"))
            {
                result += "</html>";
            }

            return result;
        }

        #endregion

        #endregion
    }
}
