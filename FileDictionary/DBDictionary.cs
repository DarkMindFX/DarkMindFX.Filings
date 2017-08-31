using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Dictionaries
{
    [Export("DB", typeof(IDictionary))]
    public class DBDictionary : IDictionary
    {
        Interfaces.DAL.IDal _dal = null;

        [ImportingConstructor]
        public DBDictionary(Interfaces.DAL.IDal dal)
        {
            _dal = dal;
        }
        public List<CompanyInfo> GetCompaniesByRegulator(string regulatorCode)
        {
            throw new NotImplementedException();
        }

        public List<RegulatorInfo> GetRegulators()
        {
            List<RegulatorInfo> result = new List<RegulatorInfo>();

            Interfaces.DAL.GetRegulatorsResult regResults = _dal.GetRegulators();
            foreach (var rInfo in regResults.Regulators)
            {
                RegulatorInfo r = new RegulatorInfo();
                r.Code = rInfo.Code;
                r.CountryCode = rInfo.CountryCode;
                r.Name = rInfo.Name;

                result.Add(r);
            }

            return result;
        }

        public string LookupRegulatorCompanyCode(string regulatorCode, string companyCode)
        {
            throw new NotImplementedException();
        }
    }
}
