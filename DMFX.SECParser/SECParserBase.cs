using DMFX.Interfaces;
using DMFX.SECParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.SECParser
{
    
    public abstract class SECParserBase : IFilingParser
    {
        #region IFilingParser implementation
        public abstract IFilingParserResult Parse(IFilingParserParams parserParams);

        #endregion

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

        protected void ExtractContexts(XmlDocument doc, SECParserResult secResult)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("xbrli", "http://www.xbrl.org/2003/instance");

            XmlNodeList contextTags = doc.GetElementsByTagName("xbrli:context");
            if (contextTags != null)
            {
                foreach (XmlNode contextTag in contextTags)
                {
                    string ID = contextTag.Attributes["id"].Value;
                    if (!ID.Contains("_"))
                    {

                        DateTime startDate = DateTime.MinValue;
                        DateTime endDate = DateTime.MinValue;
                        DateTime instant = DateTime.MinValue;

                        XmlNode tagStartDate = contextTag.SelectSingleNode("xbrli:period/xbrli:startDate", nsmgr);
                        XmlNode tagEndDate = contextTag.SelectSingleNode("xbrli:period/xbrli:endDate", nsmgr);
                        XmlNode tagInstant = contextTag.SelectSingleNode("xbrli:period/xbrli:instant", nsmgr);

                        if (tagStartDate != null)
                        {
                            startDate = DateTime.Parse(tagStartDate.InnerText);
                        }
                        if (tagEndDate != null)
                        {
                            endDate = DateTime.Parse(tagEndDate.InnerText);
                        }
                        if (tagInstant != null)
                        {
                            instant = DateTime.Parse(tagInstant.InnerText);
                        }

                        FilingContex context = new FilingContex(ID, startDate, endDate, instant);
                        secResult.Contexts.Add(context);
                    }
                }
            }
        }

       
        protected void ExtractCompanyData(XmlDocument doc, SECParserResult secResult)
        {
            string[][] tags = 
            {
                new string[] { "dei:EntityRegistrantName", "EntityRegistrantName" },
                new string[] { "dei:TradingSymbol", "TradingSymbol" },
                new string[] { "dei:EntityCentralIndexKey", "EntityCentralIndexKey" }
            };

            ExtractXmlData(doc, secResult, tags, secResult.CompanyData);
        }

        protected void ExtractFilingData(XmlDocument doc, SECParserResult secResult)
        {
            string[][] tags =
            {
                new string[] { "dei:DocumentPeriodEndDate", "DocumentPeriodEndDate" },
                new string[] { "dei:DocumentType", "DocumentType" },
                new string[] { "dei:DocumentFiscalYearFocus", "DocumentFiscalYearFocus" },
                new string[] { "dei:DocumentFiscalPeriodFocus", "DocumentFiscalPeriodFocus" },
            };

            ExtractXmlData(doc, secResult, tags, secResult.FilingData);
        }

       

        protected void ExtractXmlData(XmlDocument doc, SECParserResult secResult, string[][] tags, Dictionary<string,string> values)
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

        public IFilingParserParams CreateFilingParserParams()
        {
            return new SECParserParams();
        }
    }
}
