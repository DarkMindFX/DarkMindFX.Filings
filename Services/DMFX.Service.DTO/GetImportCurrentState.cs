using DMFX.Interfaces;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetImporterState/{SessionToken}", "GET")]
    [Route("/GetImporterState", "POST")]
    public class GetImporterState : RequestBase, IReturn<GetImporterStateResponse>
    {
    }

    public class GetImporterStateResponse : ResponseBase
    {
        public GetImporterStateResponse()
        {
            CompaniesProcessed = new List<CompanyInfo>();
        }
        public string State
        {
            get;
            set;
        }

        public DateTime? LastImportRun
        {
            get;
            set;
        }

        public DateTime? LastImportEnd
        {
            get;
            set;
        }

        public List<CompanyInfo> CompaniesProcessed
        {
            get;
            set;
        }
    }


}
