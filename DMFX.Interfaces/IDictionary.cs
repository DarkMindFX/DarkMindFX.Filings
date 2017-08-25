using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    /// <summary>
    /// Provides an access to dictionaries
    /// </summary>
    public interface IDictionary
    {
        string LookupRegulatorCompanyCode(string regulatorCode, string companyCode);
    }
}
