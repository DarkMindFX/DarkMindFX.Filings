using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMFX.Service.Sourcing
{
    public class FilingImportInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public DateTime Submitted
        {
            get;
            set;
        }
    }

    public class CompanyImportResult
    {
        public CompanyImportResult()
        {
            Filings = new List<FilingImportInfo>();
        }
        public DateTime ImportStartTime
        {
            get;
            set;
        }

        public DateTime ImportEndTime
        {
            get;
            set;
        }

        public string RegulatorCode
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public IList<FilingImportInfo> Filings
        {
            get;
            private set;
        }
    }
}