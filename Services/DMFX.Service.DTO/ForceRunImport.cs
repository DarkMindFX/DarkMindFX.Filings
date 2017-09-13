using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceRunImport/{SessionToken}", "GET")]
    [Route("/ForceRunImport/{DateStart}/{DateEnd}/{SessionToken}", "GET")]
    [Route("/ForceRunImport/{DateStart}/{DateEnd}/{RegulatorCode}/{CompanyCode}/{SessionToken}", "GET")]
    [Route("/ForceRunImport", "PUT")]
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

        public string CompanyCode
        {
            get;
            set;
        }
    }

    public class ForceRunImportResponse : ResponseBase
    {        
    }
}
