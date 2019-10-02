using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ClearLayer/{LayerCode}/{SessionToken}", "GET")]
    [Route("/ClearLayer", "PUT")]
    public class ClearLayer : RequestBase, IReturn<ClearLayerResponse>
    {
        public string LayerCode
        {
            get;
            set;
        }
    }

    public class ClearLayerResponse : ResponseBase
    {
    }
}
