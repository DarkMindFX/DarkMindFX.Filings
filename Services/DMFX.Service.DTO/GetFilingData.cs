using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetFilingData/{RegulatorCode}/{CompanyCode}/{FilingName}")]
    public class GetFilingData : RequestBase, IReturn<GetFilingDataResponse>
    {
        public GetFilingData()
        {
            Values = new List<string>();
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

        public string FilingName
        {
            get;
            set;
        }

        public List<string> Values
        {
            get;
            set;
        }
    }

    public class GetFilingDataResponse : ResponseBase
    {
        public GetFilingDataResponse()
        {
            FilingData = new Dictionary<string, decimal>();
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

        public string FilingName
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public string PeriodEnd
        {
            get;
            set;
        }

        public string Submitted
        {
            get;
            set;
        }

        public Dictionary<string, decimal> FilingData
        {
            get;
            set;
        }
    }
}
