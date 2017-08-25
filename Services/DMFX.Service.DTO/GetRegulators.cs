using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetRegulatorsList")]
    public class GetRegulators : RequestBase, IReturn<GetResulatorsResponse>
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

    public class GetResulatorsResponse : ResponseBase
    {
        public GetResulatorsResponse()
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
