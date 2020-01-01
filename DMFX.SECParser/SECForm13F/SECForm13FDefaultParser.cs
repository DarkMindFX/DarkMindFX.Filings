using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.SECParser.SECForm13F
{
    [Export("Default", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SECForm13FDefaultParser : IFilingParser
    {

        public SECForm13FDefaultParser()
        {
        }

        public string ReportType
        {
            get
            {
                return "13F-HR";
            }
        }

        public IFilingParserParams CreateFilingParserParams()
        {
            return new SECParserParams();
        }

        public IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            SECParserParams secParams = parserParams as SECParserParams;

            SECParserResult result = new SECParserResult();

            try
            {
                ValidateFiles(secParams, result);
                if (result.Success)
                {
                    XmlDocument docPrimary = OpenDocument(secParams, "primary_doc.xml");
                    XmlDocument docInfoTable = OpenDocument(secParams, "form13fInfoTable.xml");

                    ExtractFilingData(docPrimary, result);
                    ExtractGeneralData(docPrimary, result);
                    ExtractManagersData(docPrimary, result);
                    ExtractHoldingsData(docInfoTable, result);
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(EErrorCodes.ParserError, EErrorType.Error, ex.Message);
            }

            return result;
        }

        void ExtractFilingData(XmlDocument doc, SECParserResult secResult)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(doc.NameTable);
            xmlnsManager.AddNamespace("def", "http://www.sec.gov/edgar/thirteenffiler");

            XmlNode nType = doc.SelectSingleNode("//def:submissionType", xmlnsManager);
            XmlNode nPeriodEnd = doc.SelectSingleNode("//def:edgarSubmission//def:headerData//def:filerInfo//def:periodOfReport", xmlnsManager);

            string documentType = nType.InnerText;
            DateTime periodEnd = DateTime.ParseExact( nPeriodEnd.InnerText, "MM-dd-yyyy", CultureInfo.InvariantCulture);
            DateTime periodStart = periodEnd; // TODO: need to figure out whether there are any difference in annual/quarterly reporting

            secResult.FilingData.Add("DocumentType", documentType);
            secResult.FilingData.Add("DocumentPeriodStartDate", periodStart.ToString());
            secResult.FilingData.Add("DocumentPeriodEndDate", periodEnd.ToString());

        }

        void ExtractGeneralData(XmlDocument doc, SECParserResult secResult)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(doc.NameTable);
            xmlnsManager.AddNamespace("def", "http://www.sec.gov/edgar/thirteenffiler");

            XmlNode nTotalValue = doc.SelectSingleNode("//def:edgarSubmission//def:formData//def:summaryPage//def:tableValueTotal", xmlnsManager);
            XmlNode nEntityTotal = doc.SelectSingleNode("//def:edgarSubmission//def:formData//def:summaryPage//def:tableEntryTotal", xmlnsManager);

            string idTotalValue = GenerateMultivalueFactId();
            string idEntityTotal = GenerateMultivalueFactId();

            Statement statementSection = new Statement("Totals");
            secResult.Statements.Add(statementSection);

            statementSection.Records.Add(new StatementRecord(
                            "TableValueTotal",
                            Decimal.Parse(nTotalValue.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            idTotalValue

            ));

            statementSection.Records.Add(new StatementRecord(
                            "TableEntryTotal",
                            Decimal.Parse(nEntityTotal.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            idEntityTotal

            ));
        }

        void ExtractManagersData(XmlDocument doc, SECParserResult secResult)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(doc.NameTable);
            xmlnsManager.AddNamespace("def", "http://www.sec.gov/edgar/thirteenffiler");

            string xpath = "//def:edgarSubmission//def:summaryPage//def:otherManagers2Info//def:otherManager2";

            XmlNodeList nsNonDerivs = doc.SelectNodes(xpath, xmlnsManager);
            if (nsNonDerivs != null && nsNonDerivs.Count > 0)
            {
                Statement section = new Statement("Managers");
                secResult.Statements.Add(section);
                foreach (XmlNode n in nsNonDerivs)
                {
                    string factId = GenerateMultivalueFactId();

                    XmlNode nSeqNumber = n.SelectSingleNode("def:sequenceNumber", xmlnsManager);
                    XmlNode nManagerF13FileNum = n.SelectSingleNode("def:otherManager//def:form13FFileNumber", xmlnsManager);
                    XmlNode nManagerName = n.SelectSingleNode("def:otherManager//def:name", xmlnsManager);

                    if (nSeqNumber != null)
                    {
                        section.Records.Add(new StatementRecord(
                                "SequenceNumber",
                                nSeqNumber.InnerText.Trim(),
                                null,
                                secResult.PeriodStart,
                                secResult.PeriodEnd,
                                secResult.PeriodEnd,
                                null,
                                factId));
                    }

                    if (nManagerF13FileNum != null)
                    {
                        section.Records.Add(new StatementRecord(
                                "ManagerForm13FFileNumber",
                                nManagerF13FileNum.InnerText.Trim(),
                                null,
                                secResult.PeriodStart,
                                secResult.PeriodEnd,
                                secResult.PeriodEnd,
                                null,
                                factId));
                    }

                    if (nManagerName != null)
                    {
                        section.Records.Add(new StatementRecord(
                                "ManagerName",
                                nManagerName.InnerText.Trim(),
                                null,
                                secResult.PeriodStart,
                                secResult.PeriodEnd,
                                secResult.PeriodEnd,
                                null,
                                factId));
                    }
                }
            }
        }

        void ExtractHoldingsData(XmlDocument doc, SECParserResult secResult)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(doc.NameTable);
            xmlnsManager.AddNamespace("def", "http://www.sec.gov/edgar/document/thirteenf/informationtable");

            string xpath = "//def:informationTable//def:infoTable";

            XmlNodeList nsNonDerivs = doc.SelectNodes(xpath, xmlnsManager);
            if (nsNonDerivs != null && nsNonDerivs.Count > 0)
            {
                Statement section = new Statement("Holdings");
                secResult.Statements.Add(section);

                foreach (XmlNode n in nsNonDerivs)
                {
                    string factId = GenerateMultivalueFactId();

                    XmlNode nNameOfIssuer = null;
                    XmlNode nTitleOfClass = null;
                    XmlNode nCusip = null;
                    XmlNode nValue = null;
                    XmlNode nAmount = null;
                    XmlNode nType = null;
                    XmlNode nInvestmentDiscretion = null;
                    XmlNode nPutCall = null;
                    XmlNode nOtherManager = null;
                    XmlNode nVoteSole = null;
                    XmlNode nVoteShared = null;
                    XmlNode nVoteNone = null;

                    foreach (XmlNode c in n.ChildNodes)
                    {
                        
                        switch (c.Name)
                        {
                            case "nameOfIssuer":
                                nNameOfIssuer = c;
                                break;
                            case "titleOfClass":
                                nTitleOfClass = c;
                                break;
                            case "cusip":
                                nCusip = c;
                                break;
                            case "value":
                                nValue = c;
                                break;
                            case "shrsOrPrnAmt":
                                nAmount = c.SelectSingleNode("def:sshPrnamt", xmlnsManager);
                                nType = c.SelectSingleNode("def:sshPrnamtType", xmlnsManager);
                                break;
                            case "investmentDiscretion":
                                nInvestmentDiscretion = c;
                                break;
                            case "putCall":
                                nPutCall = c;
                                break;
                            case "otherManager":
                                nOtherManager = c;
                                break;
                            case "votingAuthority":
                                nVoteSole = c.SelectSingleNode("def:Sole", xmlnsManager);
                                nVoteShared = c.SelectSingleNode("def:Shared", xmlnsManager);
                                nVoteNone = c.SelectSingleNode("def:None", xmlnsManager);
                                break;

                        }
                        
                    }
                    
                    if(nNameOfIssuer != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "NameOfIssuer",
                            nNameOfIssuer.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }
                    
                    if(nTitleOfClass != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "TitleOfClass",
                            nTitleOfClass.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if(nCusip != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "CUSIP",
                            nCusip.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if(nValue != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "Value",
                            Decimal.Parse(nValue.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if(nAmount != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "Amount",
                            Decimal.Parse(nAmount.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if(nType != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "Type",
                            nType.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if(nInvestmentDiscretion != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "InvestmentDiscretion",
                            nInvestmentDiscretion.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if (nPutCall != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "PutCall",
                            nPutCall.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if (nOtherManager != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "OtherManager",
                            nOtherManager.InnerText.Trim(),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if (nVoteSole != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "VoteSole",
                            Decimal.Parse(nVoteSole.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if (nVoteShared != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "VoteShared",
                            Decimal.Parse(nVoteShared.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

                    if (nVoteNone != null)
                    {
                        section.Records.Add(new StatementRecord(
                            "VoteNone",
                            Decimal.Parse(nVoteNone.InnerText.Trim()),
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId));
                    }

#if DEBUG
                    //if (section.Records.Count > 100000) break;
#endif
                }
            }
        }

        private void ValidateFiles(SECParserParams secParams, SECParserResult secResult)
        {

            if (secParams.FileContent != null && secParams.FileContent.Count > 0)
            {
                foreach (var s in secParams.FileContent)
                {
                    if (!s.Value.CanRead)
                    {
                        secResult.Success = false;
                        secResult.AddError(EErrorCodes.FileNotFound, EErrorType.Error, string.Format("Stream {0} is unaccessable", s.Key));
                    }
                }
            }
            else
            {
                secResult.Success = false;
                secResult.AddError(EErrorCodes.FileNotFound, EErrorType.Error, "Streams were not provided");
            }
        }

        protected XmlDocument OpenDocument(SECParserParams secParams, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(secParams.FileContent[name]);

            return doc;
        }

        private string GenerateMultivalueFactId()
        {
            string result = "SECForm13F-";
            string uid = Guid.NewGuid().ToString();
            uid = Regex.Replace(uid, @"-", "");

            result += uid.ToUpper();

            return result;
        }
    }
}
