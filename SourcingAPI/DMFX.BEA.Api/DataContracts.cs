using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.BEA.Api
{
    [DataContract]
    public class BEAError
    {
        [DataMember]
        public int APIErrorCode
        {
            get;
            set;
        }

        [DataMember]
        public string APIErrorDescription
        {
            get;
            set;
        }
    }

    [DataContract]
    public class BEARequestParam
    {
        [DataMember]
        public string ParameterName
        {
            get;
            set;
        }

        [DataMember]
        public string ParameterValue
        {
            get;
            set;
        }

        public override string ToString()
        {
            return ParameterName + "=" + ParameterValue;
        }
    }

    [DataContract]
    public class BEARequest
    {

        public BEARequest()
        {
            RequestParam = new List<BEARequestParam>();
        }

        [DataMember]
        public List<BEARequestParam> RequestParam
        {
            get;
            set;
        }
    }

    [DataContract]
    public class BEAResultsBase
    {
        [DataMember]
        public BEAError Error
        {
            get;
            set;
        }
    }

    [DataContract]
    public class BEAAPI<TResults>
    {
        [DataMember]
        public BEARequest Request
        {
            get;
            set;
        }

        [DataMember]
        public TResults Results
        {
            get;
            set;
        }

    }

    [DataContract]
    public class BEAResponse<TResults>
    {
        [DataMember]
        public BEAAPI<TResults> BEAAPI
        {
            get;
            set;
        }
    }

}
