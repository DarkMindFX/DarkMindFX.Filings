using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceRunImport")]
    public class ForceRunImport : RequestBase, IReturn<ForceRunImportResponse>
    {
    }

    public class ForceRunImportResponse : ResponseBase
    {        
    }
}
