using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetCompanyFilings")]
    public class GetCompanyFilings : RequestBase, IReturn<GetCompanyFilingsResponse>
    {
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
    }

    public class GetCompanyFilingsResponse : ResponseBase
    {
        public GetCompanyFilingsResponse()
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
}
