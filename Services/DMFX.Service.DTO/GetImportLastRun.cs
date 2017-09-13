using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetImportLastRun/{SessionToken}", "GET")]
    [Route("/GetImportLastRun", "PUT")]
    public class GetImportLastRun : RequestBase, IReturn<GetImportLastRunResponse>
    {
    }

    public class GetImportLastRunResponse : ResponseBase
    {
        public DateTime LastRun
        {
            get;
            set;
        }
    }
}
