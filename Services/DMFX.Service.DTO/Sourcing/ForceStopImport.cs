using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceStopImport/{SessionToken}", "GET")]
    [Route("/ForceStopImport", "PUT")]
    public class ForceStopImport : RequestBase, IReturn<ForceStopImportResponse>
    {
        public DateTime? DateStart
        {
            get;
            set;
        }

        public DateTime? DateEnd
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
    }

    public class ForceStopImportResponse : ResponseBase
    {
    }
}
