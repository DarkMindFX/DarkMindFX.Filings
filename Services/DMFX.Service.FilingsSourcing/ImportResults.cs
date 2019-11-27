using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DMFX.Interfaces;

namespace DMFX.Service.Sourcing
{
    /// <summary>
    /// Class holds information and results about import run
    /// Each run will create the new object and the 
    /// </summary>
    public class ImportResults
    {
        private object _lockErrors = new object();
        private object _lockCompanies = new object();
        private Dictionary<string, CompanyImportResult> _companies;

        public ImportResults()
        {
            ErrorsLog = new List<Error>();
            _companies = new Dictionary<string, CompanyImportResult>();
        }

        public IList<Error> ErrorsLog
        {
            get;
            private set;
        }

        public IList<CompanyImportResult> Companies
        {
            get
            {
                return _companies.Values.ToList();
            }
        }

        public void AddError(Error error)
        {
            lock(_lockErrors)
            {
                ErrorsLog.Add(error);
            }
        }

        public void AddCompanyImportResult(CompanyImportResult compImpResult)
        {
            lock(_lockCompanies)
            {
                if (!_companies.ContainsKey(compImpResult.CompanyCode))
                {
                    _companies.Add(compImpResult.CompanyCode, compImpResult);
                }
            }
        }

        
    }
}