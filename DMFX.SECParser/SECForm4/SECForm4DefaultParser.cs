using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.SECParser.SECForm4
{
    [Export("Defult", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SECForm4DefaultParser : IFilingParser
    {

        public SECForm4DefaultParser() 
        {
        }

        public string ReportType
        {
            get
            {
                return "4";
            }
        }

        public IFilingParserParams CreateFilingParserParams()
        {
            return new SECParserParams();
        }

        public IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            ResetState();
            SECParserParams secParams = parserParams as SECParserParams;

            SECParserResult result = new SECParserResult();

            try
            {
                ValidateFile(secParams, result);
                if (result.Success)
                {
                    var doc = OpenDocument(secParams);
                    if (doc != null)
                    {                       
                        ExtractCompanyData(doc, result);
                        ExtractReportingOwnerData(doc, result);
                        ExtractFilingData(doc, result);
                        ExtractNonDerivaties(doc, result);
                        ExtractDerivaties(doc, result);
                    }
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(EErrorCodes.ParserError, EErrorType.Error, ex.Message);
            }

            return result;
        }

        #region Support methods
        private void ResetState()
        {            
        }

        protected void ValidateFile(SECParserParams secParams, SECParserResult secResult)
        {
            if (secParams.FileContent == null || !secParams.FileContent.CanRead)
            {
                secResult.Success = false;
                secResult.AddError(EErrorCodes.FileNotFound, EErrorType.Error, "Stream is unaccessable");
            }
        }

        protected XmlDocument OpenDocument(SECParserParams secParams)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(secParams.FileContent);

            return doc;
        }

        protected void ExtractXmlData(XmlDocument doc, SECParserResult secResult, string[][] tags, Dictionary<string, string> values)
        {
            foreach (var pair in tags)
            {
                XmlNodeList nodes = doc.GetElementsByTagName(pair[0]);
                if (nodes != null && nodes.Count > 0)
                {
                    values.Add(pair[1], nodes[0].InnerText);
                }

            }
        }

        void ExtractCompanyData(XmlDocument doc, SECParserResult secResult)
        {
            string[][] tags =
            {
                new string[] { "issuerName", "EntityRegistrantName" },
                new string[] { "issuerTradingSymbol", "TradingSymbol" },
                new string[] { "issuerCik", "EntityCentralIndexKey" }
            };

            ExtractXmlData(doc, secResult, tags, secResult.CompanyData);
        }
        void ExtractReportingOwnerData(XmlDocument doc, SECParserResult secResult)
        {
            string[][] tags =
            {
                new string[] { "rptOwnerName", "OwnerName" },
                new string[] { "rptOwnerCik", "OwnerCentralIndexKey" },
                new string[] { "rptOwnerStreet1", "OwnerAddressStreet1" },
                new string[] { "rptOwnerStreet2", "OwnerAddressStreet1" },
                new string[] { "rptOwnerCity", "OwnerAddressCity" },
                new string[] { "rptOwnerState", "OwnerAddressState" },
                new string[] { "rptOwnerZipCode", "OwnerAddressZipCode" },
                new string[] { "rptOwnerStateDescription", "OwnerAddressStateDesc" },
            };

            Dictionary<string, string> values = new Dictionary<string, string>();
            ExtractXmlData(doc, secResult, tags, values);

            string mvalId = GenerateMultivalueFactId();

            Statement statementSection = new Statement("reportingOwner");
            foreach (var k in values.Keys)
            {
                
                StatementRecord record = new StatementRecord(
                            k,
                            values[k],
                            string.Empty,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            mvalId
                        );
                

                if (!statementSection.Records.Contains(record))
                {
                    statementSection.Records.Add(record);
                }
                
            }

        }
        void ExtractFilingData(XmlDocument doc, SECParserResult secResult)
        {
            string[][] tags =
            {
                new string[] { "periodOfReport", "DocumentPeriodStartDate" },
                new string[] { "periodOfReport", "DocumentPeriodEndDate" },
                new string[] { "documentType", "DocumentType" },

            };

            ExtractXmlData(doc, secResult, tags, secResult.FilingData);
        }
        void ExtractNonDerivaties(XmlDocument doc, SECParserResult secResult)
        {
        }
        void ExtractDerivaties(XmlDocument doc, SECParserResult secResult)
        {
        }

        string GenerateMultivalueFactId()
        {
            string result = "SECForm4-";
            string uid = Guid.NewGuid().ToString().Remove('-');
            result += uid.ToUpper();

            return result;
        }
        #endregion
    }
}
