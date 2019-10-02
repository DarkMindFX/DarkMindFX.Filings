using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetRegulators/{SessionToken}", "GET")]
    [Route("/GetRegulators", "POST")]
    public class GetRegulators : RequestBase, IReturn<GetRegulatorsResponse>
    {
        
    }

    public class RegulatorInfo
    {
        public string Code
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }

    public class GetRegulatorsResponse : ResponseBase
    {
        public GetRegulatorsResponse()
        {
            Regulators = new List<RegulatorInfo>();
        }

        public List<RegulatorInfo> Regulators
        {
            get;
            set;
        }
    }
}
