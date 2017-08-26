using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public class RegulatorInfo
    {
        public string Code
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }
    }

    public class CompanyInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Provides an access to dictionaries
    /// </summary>
    public interface IDictionary
    {

        /// <summary>
        /// Returns company code as it is specified for specific regulator - i.e. CIK number fo SEC
        /// </summary>
        /// <param name="regulatorCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        string LookupRegulatorCompanyCode(string regulatorCode, string companyCode);

        /// <summary>
        /// Returns list of available regulators
        /// </summary>
        /// <returns></returns>
        List<RegulatorInfo> GetRegulators();

        /// <summary>
        /// Returns list of companies who files for given regulator
        /// </summary>
        /// <param name="regulatorCode"></param>
        /// <returns></returns>
        List<CompanyInfo> GetCompaniesByRegulator(string regulatorCode);
    }
}
