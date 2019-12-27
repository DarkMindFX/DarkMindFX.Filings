using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.SECParser.SECForm4
{
    [Export("Default", typeof(IFilingParser))]
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
                        ExtractFilingData(doc, result);
                        ExtractReportingOwnerData(doc, result);
                        ExtractNonDerivaties(doc, result, result.PeriodEnd);
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
            if (secParams.FileContent == null || !secParams.FileContent.Values.ElementAt(0).CanRead)
            {
                secResult.Success = false;
                secResult.AddError(EErrorCodes.FileNotFound, EErrorType.Error, "Stream is unaccessable");
            }
        }

        protected XmlDocument OpenDocument(SECParserParams secParams)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(secParams.FileContent.Values.ElementAt(0));

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
                new string[] { "rptOwnerStreet2", "OwnerAddressStreet2" },
                new string[] { "rptOwnerCity", "OwnerAddressCity" },
                new string[] { "rptOwnerState", "OwnerAddressState" },
                new string[] { "rptOwnerZipCode", "OwnerAddressZipCode" },
                new string[] { "rptOwnerStateDescription", "OwnerAddressStateDesc" },

                new string[] { "isDirector", "OwnerIsDirector" },
                new string[] { "isOfficer", "OwnerIsOfficer" },
                new string[] { "isTenPercentOwner", "Owner10PercentHolder" },
                new string[] { "isOther", "OwnerOther" },
                new string[] { "officerTitle", "OwnerOfficerTitle" },
                new string[] { "otherText", "OwnerOtherText" },
            };

            Dictionary<string, string> values = new Dictionary<string, string>();
            ExtractXmlData(doc, secResult, tags, values);

            string factId = GenerateMultivalueFactId();

            Statement statementSection = new Statement("ReportingOwner");
            foreach (var k in values.Keys)
            {
                decimal decValue = 0;
                
                StatementRecord record = new StatementRecord(
                            k,
                            Decimal.TryParse(values[k], out decValue) ? (object)decValue : values[k],
                            null,
                            secResult.PeriodStart,
                            secResult.PeriodEnd,
                            secResult.PeriodEnd,
                            null,
                            factId
                        );
                

                if (!statementSection.Records.Contains(record))
                {
                    statementSection.Records.Add(record);
                }
                
            }

            secResult.Statements.Add(statementSection);

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
        void ExtractNonDerivaties(XmlDocument doc, SECParserResult secResult, DateTime dtReportDate)
        {
            string[] xpaths = { "//nonDerivativeTable//nonDerivativeTransaction", "//nonDerivativeTable//nonDerivativeHolding" };

            foreach (var xpath in xpaths)
            {
                XmlNodeList nsNonDerivs = doc.SelectNodes(xpath);
                if (nsNonDerivs != null && nsNonDerivs.Count > 0)
                {
                    Statement section = new Statement("NonDerivativeTransactions");
                    secResult.Statements.Add(section);
                    foreach (XmlNode n in nsNonDerivs)
                    {
                        string factId = GenerateMultivalueFactId();

                        XmlNode nSecurityTitle = n.SelectSingleNode("securityTitle//value");

                        XmlNode nTransactionDate = n.SelectSingleNode("transactionDate//value");

                        XmlNode nTransactionFormType = n.SelectSingleNode("transactionCoding//transactionFormType");
                        XmlNode nTransactionFormTransCode = n.SelectSingleNode("transactionCoding//transactionCode");
                        XmlNode nTransactionFormEquitySwapInvolved = n.SelectSingleNode("transactionCoding//equitySwapInvolved");

                        XmlNode nTransactionAmountShares = n.SelectSingleNode("transactionAmounts//transactionShares//value");
                        XmlNode nTransactionAmountPrice = n.SelectSingleNode("transactionAmounts//transactionPricePerShare//value");
                        XmlNode nTransactionAmountADCode = n.SelectSingleNode("transactionAmounts//transactionAcquiredDisposedCode//value");

                        XmlNode nPostShares = n.SelectSingleNode("postTransactionAmounts//sharesOwnedFollowingTransaction//value");

                        XmlNode nPostOwnership = n.SelectSingleNode("ownershipNature//directOrIndirectOwnership//value");

                        XmlNode nOwnershipNature = n.SelectSingleNode("ownershipNature//natureOfOwnership//value");

                        DateTime dtDate = dtReportDate;
                        if (nTransactionDate != null)
                        {
                            dtDate = DateTime.Parse(nTransactionDate.InnerText);
                        }

                        StatementRecord rIsDerivTrans = new StatementRecord(
                                    "IsDerivTrans",
                                    (decimal)0.0,
                                    null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rIsDerivTrans);

                        if (nSecurityTitle != null)
                        {
                            StatementRecord rSecurityTitle = new StatementRecord(
                                    "SecurityTitle",
                                    nSecurityTitle.InnerText,
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rSecurityTitle);
                        }

                        if (nTransactionFormType != null)
                        {
                            StatementRecord rFormType = new StatementRecord(
                                    "FormType",
                                    nTransactionFormType.InnerText,
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rFormType);
                        }

                        if (nTransactionFormTransCode != null)
                        {
                            StatementRecord rFormTransCode = new StatementRecord(
                                    "TransactionCode",
                                    nTransactionFormTransCode.InnerText,
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rFormTransCode);
                        }

                        if (nTransactionFormEquitySwapInvolved != null)
                        {
                            decimal dEquitySwapInvolved = 0;
                            if(!Decimal.TryParse(nTransactionFormEquitySwapInvolved.InnerText, out dEquitySwapInvolved))
                            {
                                bool bEquitySwapInvolved = false;
                                bool.TryParse(nTransactionFormEquitySwapInvolved.InnerText, out bEquitySwapInvolved);

                                dEquitySwapInvolved = bEquitySwapInvolved ? 1 : 0;

                            }
                            StatementRecord rFormEquitySwapInv = new StatementRecord(
                                    "EquitySwapInvolved",
                                    dEquitySwapInvolved,
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rFormEquitySwapInv);
                        }

                        if (nTransactionAmountShares != null)
                        {
                            StatementRecord rTransShares = new StatementRecord(
                                    "TransactionShares",
                                    (nTransactionAmountADCode != null && nTransactionAmountADCode.InnerText.ToUpper().Equals("D") ? -1 : 1) * Decimal.Parse(nTransactionAmountShares.InnerText),
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rTransShares);
                        }

                        if (nTransactionAmountPrice != null)
                        {

                            StatementRecord rTransPrice = new StatementRecord(
                                "TransactionPrice",
                                Decimal.Parse(nTransactionAmountPrice.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rTransPrice);
                        }

                        if (nPostShares != null)
                        {
                            StatementRecord rPostShares = new StatementRecord(
                                    "PostTransactionSharesOwnership",
                                    Decimal.Parse(nPostShares.InnerText),
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rPostShares);
                        }

                        if (nPostOwnership != null)
                        {
                            StatementRecord rPostOwnership = new StatementRecord(
                                    "PostTransactionDirectOrIndirectOwnership",
                                    nPostOwnership.InnerText,
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rPostOwnership);
                        }

                        if (nOwnershipNature != null)
                        {
                            StatementRecord rOwnershipNature = new StatementRecord(
                                    "PostTransactionOwnershipNature",
                                    nOwnershipNature.InnerText,
                                    null, dtDate, dtDate, dtDate, null, factId);
                            section.Records.Add(rOwnershipNature);
                        }

                    }
                }
            }

        }

        void ExtractDerivaties(XmlDocument doc, SECParserResult secResult)
        {
            string xpath = "//derivativeTable//derivativeTransaction";

            XmlNodeList nsNonDerivs = doc.SelectNodes(xpath);
            if (nsNonDerivs != null && nsNonDerivs.Count > 0)
            {
                Statement section = new Statement("DerivativeTransactions");
                secResult.Statements.Add(section);
                foreach (XmlNode n in nsNonDerivs)
                {
                    string factId = GenerateMultivalueFactId();

                    XmlNode nSecurityTitle = n.SelectSingleNode("securityTitle//value");
                    XmlNode nConvExPrice = n.SelectSingleNode("conversionOrExercisePrice//value");

                    XmlNode nTransactionDate = n.SelectSingleNode("transactionDate//value");

                    XmlNode nTransactionFormType = n.SelectSingleNode("transactionCoding//transactionFormType");
                    XmlNode nTransactionFormTransCode = n.SelectSingleNode("transactionCoding//transactionCode");
                    XmlNode nTransactionFormEquitySwapInvolved = n.SelectSingleNode("transactionCoding//equitySwapInvolved");

                    XmlNode nTransactionAmountShares = n.SelectSingleNode("transactionAmounts//transactionShares//value");
                    XmlNode nTransactionAmountPrice = n.SelectSingleNode("transactionAmounts//transactionPricePerShare//value");
                    XmlNode nTransactionAmountADCode = n.SelectSingleNode("transactionAmounts//transactionAcquiredDisposedCode//value");

                    XmlNode nExDate = n.SelectSingleNode("exerciseDate//value");
                    XmlNode nExpDate = n.SelectSingleNode("expirationDate//value");

                    XmlNode nUnderSecurityTitle = n.SelectSingleNode("underlyingSecurity//underlyingSecurityTitle//value");
                    XmlNode nUnderSecurityShares = n.SelectSingleNode("underlyingSecurity//underlyingSecurityShares//value");

                    XmlNode nPostShares = n.SelectSingleNode("postTransactionAmounts//sharesOwnedFollowingTransaction//value");

                    XmlNode nPostOwnership = n.SelectSingleNode("ownershipNature//directOrIndirectOwnership//value");

                    DateTime dtDate = DateTime.Parse(nTransactionDate.InnerText);

                    StatementRecord rIsDerivTrans = new StatementRecord(
                                "IsDerivTrans",
                                (decimal)1.0,
                                null, dtDate, dtDate, dtDate, null, factId);
                    section.Records.Add(rIsDerivTrans);

                    if (nSecurityTitle != null)
                    {
                        StatementRecord rSecurityTitle = new StatementRecord(
                                "SecurityTitle",
                                nSecurityTitle.InnerText,
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rSecurityTitle);
                    }

                    if (nConvExPrice != null)
                    {
                        StatementRecord rConvExPrice = new StatementRecord(
                            "ConversionOrExercisePrice",
                            Decimal.Parse(nConvExPrice.InnerText),
                            null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rConvExPrice);
                    }

                    if (nTransactionFormType != null)
                    {
                        StatementRecord rFormType = new StatementRecord(
                                "FormType",
                                nTransactionFormType.InnerText,
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rFormType);
                    }

                    if (nTransactionFormTransCode != null)
                    {
                        StatementRecord rFormTransCode = new StatementRecord(
                                "TransactionCode",
                                nTransactionFormTransCode.InnerText,
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rFormTransCode);
                    }

                    if (nTransactionFormEquitySwapInvolved != null)
                    {
                        StatementRecord rFormEquitySwapInv = new StatementRecord(
                                "EquitySwapInvolved",
                                Decimal.Parse(nTransactionFormEquitySwapInvolved.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rFormEquitySwapInv);
                    }

                    if (nTransactionAmountShares != null)
                    {
                        StatementRecord rTransShares = new StatementRecord(
                                "TransactionShares",
                                (nTransactionAmountADCode != null && nTransactionAmountADCode.InnerText.ToUpper().Equals("D") ? -1 : 1) * Decimal.Parse(nTransactionAmountShares.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rTransShares);
                    }

                    if (nTransactionAmountPrice != null)
                    {
                        StatementRecord rTransPrice = new StatementRecord(
                                "TransactionPrice",
                                Decimal.Parse(nTransactionAmountPrice.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rTransPrice);
                    }

                    
                    if (nExDate != null)
                    {
                        StatementRecord rExDate = new StatementRecord(
                                "ExerciseDate",
                                DateTime.Parse(nExDate.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rExDate);
                    }

                    if (nExpDate != null)
                    {
                        StatementRecord rExpDate = new StatementRecord(
                                "ExpirationDate",
                                DateTime.Parse(nExpDate.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rExpDate);
                    }

                    if (nUnderSecurityTitle != null)
                    {
                        StatementRecord rUnderSecurityTitle = new StatementRecord(
                                "UnderlyingSecurityTitle",
                                nUnderSecurityTitle.InnerText,
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rUnderSecurityTitle);
                    }

                    if (nUnderSecurityShares != null)
                    {
                        StatementRecord rUnderSecurityShares = new StatementRecord(
                                "UnderlyingSecurityShares",
                                Decimal.Parse(nUnderSecurityShares.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rUnderSecurityShares);
                    }

                    if (nPostShares != null)
                    {
                        StatementRecord rPostShares = new StatementRecord(
                                "PostTransactionSharesOwnership",
                                Decimal.Parse(nPostShares.InnerText),
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rPostShares);
                    }

                    if (nPostOwnership != null)
                    {
                        StatementRecord rPostOwnership = new StatementRecord(
                                "PostTransactionDirectOrIndirectOwnership",
                                nPostOwnership.InnerText,
                                null, dtDate, dtDate, dtDate, null, factId);
                        section.Records.Add(rPostOwnership);
                    }

                }
            }
        }

        string GenerateMultivalueFactId()
        {
            string result = "SECForm4-";
            string uid = Guid.NewGuid().ToString();
            uid = Regex.Replace(uid, @"-", "");
            
            result += uid.ToUpper();

            return result;
        }
        #endregion
    }
}
