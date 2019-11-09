using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceRunImport/{RegulatorCode}/{SessionToken}", "GET")]
    [Route("/ForceRunImport/{DaysBack}/{RegulatorCode}/{SessionToken}", "GET")]
    [Route("/ForceRunImport/{DaysBack}/{RegulatorCode}/{CompanyCodes}/{SessionToken}", "GET")]
    [Route("/ForceRunImport/{DateStart}/{DateEnd}/{RegulatorCode}/{CompanyCodes}/{SessionToken}", "GET")]
    [Route("/ForceRunImport", "POST")]
    public class ForceRunImport : RequestBase, IReturn<ForceRunImportResponse>
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

        public string[] CompanyCodes
        {
            get;
            set;
        }

        public decimal? DaysBack
        {
            get;
            set;
        }
    }

    public class ForceRunImportResponse : ResponseBase
    {        
    }
}
