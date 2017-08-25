using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DMFX.Dictionaries
{
    [Export(typeof(IDictionary))]
    public class FileDictionary : IDictionary
    {
        class SECCompany
        {
            public string CIK
            {
                get;
                set;
            }

            public string Ticker
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        Dictionary<string, SECCompany> _lookupSECCompanies = null;
        Dictionary<string, SECCompany> _lookupSECCompaniesByTicker = null;

        public FileDictionary()
        {
            Init();
        }

        #region Support methods

        void Init()
        {
            _lookupSECCompanies = new Dictionary<string, SECCompany>();
            _lookupSECCompaniesByTicker = new Dictionary<string, SECCompany>();

            ReadXMLs();
        }

        void ReadXMLs()
        {
            // Reading SEC list
            ReadSEC();


        }

        void ReadSEC()
        {
            string secCompanies = Resources.SECCompanyList;
            XmlDocument doc = new XmlDocument();
 
            doc.LoadXml(secCompanies);

            var companiesTags = doc.SelectNodes("/companies/company");
            if (companiesTags != null)
            {
                for(int i = 0; i < companiesTags.Count; ++i)
                {
                    SECCompany comp = new SECCompany();
                    comp.CIK = companiesTags[i].Attributes["cik"].Value;
                    comp.Ticker = companiesTags[i].Attributes["ticker"].Value;
                    comp.Name = companiesTags[i].Attributes["name"].Value;

                    _lookupSECCompanies.Add(comp.CIK, comp);
                    _lookupSECCompaniesByTicker.Add(comp.Ticker, comp);

                }
            }
            
        }

        #endregion

        public string LookupRegulatorCompanyCode(string regulatorCode, string companyCode)
        {
            string code = string.Empty;

            code = _lookupSECCompaniesByTicker[companyCode].CIK;

            return code;
        }
    }
}
