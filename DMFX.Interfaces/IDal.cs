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

    public interface IDal
    {
        void Init(IDalParams dalParams);

        IDalParams CreateDalParams();

        void InsertFilingDetails(InsertFilingDetailsParams filingDetails);
    }
}
