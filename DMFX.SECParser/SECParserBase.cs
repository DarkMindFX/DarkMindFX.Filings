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
        private string _reportType = string.Empty;

        public SECParserBase(string reportType)
        {
            _reportType = reportType;
        }
        #region IFilingParser implementation
        public abstract IFilingParserResult Parse(IFilingParserParams parserParams);

        public string ReportType
        {
            get
            {
                return _reportType;
            }
        }

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

        protected void ParseStatementSection(XmlDocument doc, SECParserResult result, string sectionTitle, string[][] sectionTags)
        {
            XmlNamespaceManager nsmgr = PrepareNamespaceMngr(doc);

            // preparing statements
            Statement statementSection = new Statement(sectionTitle);
            foreach (var tag in sectionTags)
            {
                foreach (var context in result.Contexts)
                {
                    var contextAtt = new Dictionary<string, string>();
                    contextAtt.Add("contextRef", context.ID + (tag.Length > 2 ? tag[2] : string.Empty));
                    XmlNode valueTag = FindNode(doc, tag[0], contextAtt);

                    if (valueTag != null)
                    {
                        StatementRecord record = new StatementRecord(
                            tag[1],
                            Decimal.Parse(valueTag.InnerText),
                            valueTag.Attributes["unitRef"].Value,
                            context.StartDate,
                            context.EndDate,
                            context.Instant,
                            valueTag.Attributes["id"].Value
                        );
                        statementSection.Records.Add(record);
                    }
                }
            }

            result.Statements.Add(statementSection);
        }

        protected XmlNode FindNode(XmlNode parent, string tag, Dictionary<string, string> attrs)
        {
            XmlNode result = null;

            foreach (XmlNode nd in parent.ChildNodes)
            {
                if (nd.Name == tag)
                {

                    int attrsCount = 0;
                    foreach (XmlAttribute attr in nd.Attributes)
                    {
                        if (attrs.ContainsKey(attr.Name) && attrs[attr.Name].Equals(attr.Value))
                        {
                            ++attrsCount;
                        }
                    }
                    if (attrsCount == attrs.Count)
                    {
                        result = nd;
                        break;
                    }
                }

                result = FindNode(nd, tag, attrs);
            }

            return result;
        }

        public IFilingParserParams CreateFilingParserParams()
        {
            return new SECParserParams();
        }

        protected virtual XmlNamespaceManager PrepareNamespaceMngr(XmlDocument doc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("us-gaap", "http://fasb.org/us-gaap/2017-01-31");
            nsmgr.AddNamespace("aapl", "http://www.apple.com/20170701");

            return nsmgr;
        }


    }
}
