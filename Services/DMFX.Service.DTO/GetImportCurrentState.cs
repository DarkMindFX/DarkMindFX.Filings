using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetImportCurrentState")]
    public class GetImportCurrentState : RequestBase, IReturn<GetImportCurrentStateResponse>
    {
    }

    public class GetImportCurrentStateResponse : ResponseBase
    {
        public string State
        {
            get;
            set;
        }
    }


}
