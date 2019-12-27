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

            public string Type
            {
                get;
                set;
            }
        }
        public class Section
        {
            public Section()
            {
                ValueTags = new Dictionary<string, ValueTag>();
            }

            public string Name
            {
                get;
                set;
            }

            public Dictionary<string, ValueTag> ValueTags
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
                        InitNsManager(doc);
                        ExtractContexts(doc, result);
                        ExtractCompanyData(doc, result);
                        ExtractFilingData(doc, result);
                        ExtractValues(doc, secParams, result);
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

        private void ResetState()
        {
            _reportType = string.Empty;
            _companyXml = null;
            _nsmgr = null;
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
            var sections = doc.SelectNodes("/parser/sections/section");

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (var ns in _namespaces.Keys)
            {
                nsmgr.AddNamespace(ns, _namespaces[ns]);
            }

            foreach (XmlNode sNode in sections)
            {
                Section section = null;
                if (!_sections.ContainsKey(sNode.Attributes["name"].Value))
                {
                    section = new Section() { Name = sNode.Attributes["name"].Value };
                    _sections.Add(section.Name, section);
                }
                else
                {
                    section = _sections[sNode.Attributes["name"].Value];
                }

                var nodes = doc.SelectNodes("//xs:element[not(@abstract='true')]", nsmgr);
                if (nodes != null)
                {                    

                    foreach (XmlNode vtNode in nodes)
                    {

                        if (!string.IsNullOrEmpty(vtNode.Attributes["name"].Value) && !string.IsNullOrEmpty(vtNode.Attributes["id"].Value))
                        {
                            string tag = vtNode.Attributes["id"].Value.Replace("us-gaap_", "us-gaap:");
                            ValueTag vt = new ValueTag()
                            {

                                Tag = tag,
                                Code = vtNode.Attributes["name"].Value,
                                Suffix = vtNode.Attributes["suffix"] != null ? vtNode.Attributes["suffix"].Value : string.Empty, // TODO: To remove
                                Type = vtNode.Attributes["type"] != null ? vtNode.Attributes["type"].Value : string.Empty

                            };
                            if (!section.ValueTags.ContainsKey(vt.Code))
                            {
                                section.ValueTags.Add(vt.Code, vt);
                            }
                        }


                    }
                }
            }
        }

        private void InitNsManager(XmlDocument doc)
        {
            if (_nsmgr != null)
            {
                _nsmgr = null;
            }

            _nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (var ns in _namespaces.Keys)
            {
                _nsmgr.AddNamespace(ns, _namespaces[ns]);
            }
            _nsmgr.AddNamespace("df", doc.DocumentElement.NamespaceURI);

            // extracting namespaces
            XmlNode xblrNode = doc.SelectSingleNode("/*[local-name()='xbrl']", _nsmgr);
            if (xblrNode != null)
            {
                foreach (XmlAttribute attr in xblrNode.Attributes)
                {
                    if (attr.Name.Contains("xmlns") && !attr.LocalName.Equals("xmlns"))
                    {
                        _nsmgr.AddNamespace(attr.LocalName, attr.Value);
                    }
                }
            }

        }
        #endregion

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

        protected void ExtractContexts(XmlDocument doc, SECParserResult secResult)
        {

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("xbrli", "http://www.xbrl.org/2003/instance");
            nsmgr.AddNamespace("xbrll", "http://www.xbrl.org/2003/linkbase");
            nsmgr.AddNamespace("df", doc.DocumentElement.NamespaceURI);

            Dictionary<string, List<string>> tags = new Dictionary<string, List<string>>();
            tags.Add("xbrli:context", new List<string>(new string[] { "xbrli:period", "xbrli:startDate", "xbrli:endDate", "xbrli:instant", "xbrli:entity/xbrli:segment" }));
            tags.Add("context", new List<string>(new string[] { "df:period", "df:startDate", "df:endDate", "df:instant", "df:entity/df:segment" }));

            string currContextTagName = string.Empty;
            XmlNodeList contextTags = null;
            int currKey = 0;
            do
            {
                currContextTagName = tags.Keys.ElementAt(currKey);
                contextTags = doc.GetElementsByTagName(currContextTagName); // "xbrli:context"

                // TODO: This need to be rewritten with Xpath requests - currently Xpath doesn't work on expressions like period/startDate for unknown reason
                if (contextTags != null)
                {

                    foreach (XmlNode contextTag in contextTags)
                    {
                        if (contextTag.SelectSingleNode(tags[currContextTagName][4], nsmgr) == null)
                        {
                            string ID = contextTag.Attributes["id"].Value;

                            DateTime startDate = DateTime.MinValue;
                            DateTime endDate = DateTime.MinValue;
                            DateTime instant = DateTime.MinValue;

                            XmlNode tagStartDate = contextTag.SelectSingleNode(tags[currContextTagName][0] + "/" + tags[currContextTagName][1], nsmgr); // "xbrli:period/xbrli:startDate"
                            XmlNode tagEndDate = contextTag.SelectSingleNode(tags[currContextTagName][0] + "/" + tags[currContextTagName][2], nsmgr); // "xbrli:period/xbrli:endDate"
                            XmlNode tagInstant = contextTag.SelectSingleNode(tags[currContextTagName][0] + "/" + tags[currContextTagName][3], nsmgr); // "xbrli:period/xbrli:instant"

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

                ++currKey;


            }
            while (currKey < tags.Keys.Count);


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
                    if (ctx.StartDate != DateTime.MinValue &&
                        (endDate - ctx.StartDate).Days >= (type == "10-Q" ? 80/*10-Q*/ : 350/*10-K*/) &&
                        (endDate - ctx.StartDate).Days <= (type == "10-Q" ? 100/*10-Q*/ : 370/*10-K*/)
                       )
                    {
                        secResult.FilingData["DocumentPeriodStartDate"] = ctx.StartDate.ToShortDateString();
                        break;
                    }
                }
            }
        }

        protected void ExtractValues(XmlDocument doc, SECParserParams secParams, SECParserResult secResult)
        {
            foreach (var s in _sections.Values)
            {
                ParseStatementSection(doc, secParams, secResult, s);
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

        protected void ParseStatementSection(XmlDocument doc, SECParserParams secParams, SECParserResult result, Section section)
        {
            // preparing statements
            Statement statementSection = new Statement(section.Name);
            foreach (var value in section.ValueTags.Values)
            {

                foreach (var context in result.Contexts)
                {
                    string xpath = "//" + value.Tag + "[@contextRef='" + (context.ID + (!string.IsNullOrEmpty(value.Suffix) ? value.Suffix : string.Empty)) + "']";
                    XmlNode valueTag = doc.SelectSingleNode(xpath, _nsmgr);

                    if (valueTag != null)
                    {
                        object valObject = null;
                        string innerText = valueTag.InnerText.Trim();
                        Decimal valDecimal;
                        DateTime valDateTime;
                        if (Decimal.TryParse(innerText, out valDecimal) && secParams.ExtractDates)
                        {
                            valObject = valDecimal;
                        }
                        else
                        {
                            if (DateTime.TryParse(innerText, out valDateTime) && secParams.ExtractDates)
                            {
                                valObject = valDateTime;
                            }
                            else
                            {
                                if (secParams.ExtractStrings)
                                {
                                    valObject = innerText;
                                }
                            }
                        }

                        if (valObject != null)
                        {
                            StatementRecord record = new StatementRecord(
                                value.Code,
                                valObject,
                                valueTag.Attributes["unitRef"].Value,
                                context.StartDate,
                                context.EndDate,
                                context.Instant,
                                valueTag.Attributes["id"] != null ? valueTag.Attributes["id"].Value : null
                            );

                            if (!statementSection.Records.Contains(record))
                            {
                                statementSection.Records.Add(record);
                            }
                        }
                    }
                }
            }

            result.Statements.Add(statementSection);
        }

        public IFilingParserParams CreateFilingParserParams()
        {
            return new SECParserParams();
        }


    }
}
