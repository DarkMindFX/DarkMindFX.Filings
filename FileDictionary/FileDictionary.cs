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

        public List<RegulatorInfo> GetRegulators()
        {
            List<RegulatorInfo> regulators = new List<RegulatorInfo>();

            // TODO: need to read this data from DAL
            regulators.Add(new RegulatorInfo() { Code = "SEC", Name = "Securities & Exchange Commision", CountryCode = "US" });
            regulators.Add(new RegulatorInfo() { Code = "FCA", Name = "Financial Conduct Authority", CountryCode = "UK" });
            regulators.Add(new RegulatorInfo() { Code = "BaFin", Name = "Bundesanstalt für Finanzdienstleistungsaufsicht", CountryCode = "DE" });
            regulators.Add(new RegulatorInfo() { Code = "FSA", Name = "Financial Services Agency", CountryCode = "JP" });

            return regulators;
        }

        public List<CompanyInfo> GetCompaniesByRegulator(string regulatorCode)
        {
            List<CompanyInfo> result = new List<CompanyInfo>();

            if (regulatorCode == "SEC")
            {
                // TODO: for now returning only SEC companies - need to implement reading from DB
                foreach (var company in _lookupSECCompanies.Values)
                {
                    CompanyInfo cmpInfo = new CompanyInfo() { Name = company.Name, Code = company.Ticker };
                    result.Add(cmpInfo);
                }
            }

            return result;
        }
    }
}
