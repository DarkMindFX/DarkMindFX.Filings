using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public interface IDalParams
    {
        Dictionary<string, string> Parameters
        {
            get;
        }
    }

    public class InsertFilingDetailsParams
    {
        public struct FilingMetadaRecord
        {
            public string Name
            {
                get;
                set;
            }

            public string Value
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
        public struct FilingRecord
        {
            public string Code
            {
                get;
                set;
            }

            public Decimal Value
            {
                get;
                set;
            }

            public DateTime PeriodStart
            {
                get;
                set;
            }

            public DateTime PeriodEnd
            {
                get;
                set;
            }

            public DateTime Instant
            {
                get;
                set;
            }

            public string Unit
            {
                get;
                set;
            }
        }

        public InsertFilingDetailsParams()
        {
            Metadata = new List<FilingMetadaRecord>();
            Data = new List<FilingRecord>();
        }

        public List<FilingMetadaRecord> Metadata
        {
            get;
            set;
        }

        public List<FilingRecord> Data
        {
            get;
            set;
        }

    }

    public class GetCompanyFilingsInfoParams
    {
        public GetCompanyFilingsInfoParams()
        {
            Types = new List<string>();
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

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public List<string> Types
        {
            get;
            set;
        }
    }


    public class CompanyFilingInfo
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

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
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

    public class GetCompanyFilingsInfoResult
    {
        public GetCompanyFilingsInfoResult()
        {
            Filings = new List<CompanyFilingInfo>();
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

        public List<CompanyFilingInfo> Filings
        {
            get;
            set;
        }
    }

    public interface IDal
    {
        void Init(IDalParams dalParams);

        IDalParams CreateDalParams();

        /// <summary>
        /// Inserts filing data into storage
        /// </summary>
        /// <param name="filingDetails"></param>
        void InsertFilingDetails(InsertFilingDetailsParams filingDetails);




    }
}
