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
        public class ValueTag
        {
            public string Tag
            {
                get;
                set;
            }

            public string Code
            {
                get;
                set;
            }

            public string Suffix
            {
                get;
                set;
            }
        }
        public class Section
        {
            public Section()
            {
                ValueTags = new List<ValueTag>();
            }

            public string Name
            {
                get;
                set;
            }

            public List<ValueTag> ValueTags
            {
                get;
                set;
            }
        }

        private string _reportType = string.Empty;

        private string _companyXml = null;

        private XmlNamespaceManager _nsmgr = null;

        private Dictionary<string, string> _namespaces = new Dictionary<string, string>();

        private Dictionary<string, Section> _sections = new Dictionary<string, Section>();

        public SECParserBase(string reportType, string companyXml = null)
        {
            _reportType = reportType;
            _companyXml = companyXml;

            InitCommon();
            InitCompany();

        }
        #region IFilingParser implementation
        public virtual IFilingParserResult Parse(IFilingParserParams parserParams)
        {
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
                        InitNsManager(doc);
                        ExtractContexts(doc, result);
                        ExtractCompanyData(doc, result);
                        ExtractFilingData(doc, result);
                        ExtractValues(doc, result);
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

        public string ReportType
        {
            get
            {
                return _reportType;
            }
        }



        #endregion

        #region Init methods
        private void InitCommon()
        {
            InitFromXml(Resources.SECCommonTags);
        }

        private void InitCompany()
        {
            if (!string.IsNullOrEmpty(_companyXml))
            {
                InitFromXml(_companyXml);
            }
        }

        private void InitFromXml(string xml)
        {
            XmlDocument settings = new XmlDocument();
            settings.LoadXml(xml);

            InitNamespacesList(settings);
            InitSectionsList(settings);
        }


        private void InitNamespacesList(XmlDocument doc)
        {
            var nodes = doc.SelectNodes("/parser/namespaces/namespace");
            if (nodes != null)
            {
                foreach (XmlNode nsNode in nodes)
                {
                    _namespaces.Add(nsNode.Attributes["ns"].Value, nsNode.Attributes["uri"].Value);
                }
            }
        }

        private void InitSectionsList(XmlDocument doc)
        {
            var nodes = doc.SelectNodes("/parser/sections/section");
            if (nodes != null)
            {
                foreach (XmlNode sNode in nodes)
                {
                    // getting / creating section
                    Section section = null;
                    if (_sections.ContainsKey(sNode.Attributes["name"].Value))
                    {
                        section = _sections[sNode.Attributes["name"].Value];
                    }
                    else
                    {
                        section = new Section() { Name = sNode.Attributes["name"].Value };
                        _sections.Add(section.Name, section);
                    }

                    // filling value tags info records
                    foreach (XmlNode vtNode in sNode.ChildNodes)
                    {
                        if (vtNode.Name == "value" && !string.IsNullOrEmpty(vtNode.Attributes["tag"].Value) && !string.IsNullOrEmpty(vtNode.Attributes["code"].Value))
                        {
                            ValueTag vt = new ValueTag()
                            {
                                Tag = vtNode.Attributes["tag"].Value,
                                Code = vtNode.Attributes["code"].Value,
                                Suffix = vtNode.Attributes["suffix"] != null ? vtNode.Attributes["suffix"].Value : string.Empty

                            };
                            section.ValueTags.Add(vt);
                        }

                    }
                }
            }
        }

        private void InitNsManager(XmlDocument doc)
        {
            if (_nsmgr == null)
            {
                _nsmgr = new XmlNamespaceManager(doc.NameTable);
                foreach (var ns in _namespaces.Keys)
                {
                    _nsmgr.AddNamespace(ns, _namespaces[ns]);
                }
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

            // separately extracting end date - using contexts
            if (secResult.FilingData.ContainsKey("DocumentPeriodEndDate") && secResult.FilingData.ContainsKey("DocumentType"))
            {
                DateTime endDate = DateTime.Parse(secResult.FilingData["DocumentPeriodEndDate"]);
                string type = secResult.FilingData["DocumentType"];

                foreach (var ctx in secResult.Contexts)
                {
                    // TODO: WARNING! this supports only 10-K and 10-Q report types - need to change in future for other types of reports
                    if (ctx.EndDate == endDate && (type == "10-Q" ? ctx.ID.Contains("QTD") : ctx.ID.Contains("YTD")))
                    {
                        secResult.FilingData["DocumentPeriodStartDate"] = ctx.StartDate.ToShortDateString();
                        break;
                    }
                }
            }
        }

        protected void ExtractValues(XmlDocument doc, SECParserResult secResult)
        {
            foreach (var s in _sections.Values)
            {
                ParseStatementSection(doc, secResult, s);
            }
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

        protected void ParseStatementSection(XmlDocument doc, SECParserResult result, Section section)
        {
            InitNamespacesList(doc);

            // preparing statements
            Statement statementSection = new Statement(section.Name);
            foreach (var value in section.ValueTags)
            {
                foreach (var context in result.Contexts)
                {
                    var contextAtt = new Dictionary<string, string>();
                    contextAtt.Add("contextRef", context.ID + (!string.IsNullOrEmpty(value.Suffix) ? value.Suffix : string.Empty));
                    XmlNode valueTag = FindNode(doc, value.Tag, contextAtt);

                    if (valueTag != null)
                    {
                        StatementRecord record = new StatementRecord(
                            value.Code,
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


    }
}
