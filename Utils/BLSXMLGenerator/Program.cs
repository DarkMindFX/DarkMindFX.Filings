using BLSXMLGenerator.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BLSXMLGenerator
{
    class Program
    {
        class TickerInfo
        {
            public TickerInfo()
            {
                Metadata = new Dictionary<string, string>();
            }

            public string Ticker_Symbol
            {
                get;
                set;
            }

            public string Ticker_Name
            {
                get;
                set;
            }

            public string Bls_Code
            {
                get;
                set;
            }

            public Dictionary<string, string> Metadata
            {
                get;
                set;
            }

        }

        static Dictionary<string, TickerInfo> _tickers = new Dictionary<string, TickerInfo>();

        static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            

            FillCodes();

            string url = "https://beta.bls.gov/dataQuery/find?st=0&r=20&s=popularity%3AD&q={0}&more=0";

            foreach (var code in _tickers.Keys.ToList())
            {
                string content = string.Empty;
                try
                {
                    string search = string.Format(url, code);
                    using (var client = new WebClient())
                    {
                        content = client.DownloadString(search);

                        int start = content.IndexOf(string.Format("<div id=\"catalog_{0}\"", code));
                        int end = content.IndexOf("</div>", start);
                        content = content.Substring(start, end + "</div>".Length - start);
                        content = content.Replace("&", " - ");

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(content);

                        XmlNode root = xmlDoc.SelectSingleNode(string.Format("//*[@id=\"catalog_{0}\"]", code));
                        if(root != null)
                        {
                            TickerInfo ti = new TickerInfo();

                            _tickers[code] = ti; ;



                            var values = root.SelectNodes("table/tbody/tr");
                            foreach(XmlNode v in values)
                            {
                                var th = v.SelectSingleNode("th");
                                var td = v.SelectSingleNode("td");

                                ti.Metadata.Add(th.InnerText.Trim(), td.InnerText.Trim());
                            }

                            ti.Bls_Code = code;
                            ti.Ticker_Name = ti.Metadata["Series Title"];
                            ti.Ticker_Symbol = "BLS." + ti.Bls_Code;
                        }

                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to load {0}, Error: {1}", code, ex.Message));
                }
                

            }

            DumpXML("BLSIndicatorsList.xml");
        }

        static void DumpXML(string path)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode indicators = doc.CreateNode(XmlNodeType.Element, "indicators", string.Empty);
            doc.AppendChild(indicators);

            foreach(var t in _tickers.Values)
            {
                if(t != null)
                {
                    XmlNode indicator = doc.CreateNode(XmlNodeType.Element, "indicator", string.Empty);
                    indicators.AppendChild(indicator);


                    XmlAttribute attrTickerSymbol = doc.CreateAttribute("ticker_symbol");
                    attrTickerSymbol.Value = t.Ticker_Symbol;

                    XmlAttribute attrTickerName = doc.CreateAttribute("name");
                    attrTickerName.Value = t.Ticker_Name;

                    XmlAttribute attrBlsCode = doc.CreateAttribute("bls_code");
                    attrBlsCode.Value = t.Bls_Code;

                    string metadata = string.Empty;
                    foreach(var k in t.Metadata.Keys)
                    {
                        metadata += k + "=" + t.Metadata[k] + ";";
                    }

                    XmlAttribute attrMeta = doc.CreateAttribute("metadata");
                    attrMeta.Value = metadata;

                    indicator.Attributes.Append(attrTickerSymbol);
                    indicator.Attributes.Append(attrBlsCode);
                    indicator.Attributes.Append(attrTickerName);
                    indicator.Attributes.Append(attrMeta);

                }
            }

            doc.Save(path);
        }

        static void FillCodes()
        {
            string txtCodes = Resources.BLSCodes;
            string[] codes = txtCodes.Split(new char[] { '\n' });

            foreach(var c in codes)
            {
                string key = c.Trim();
                if(!string.IsNullOrEmpty(key) && !_tickers.ContainsKey(key))
                {                    
                    _tickers.Add(key, null);
                }
            }
        }
    }
}
