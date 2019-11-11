using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.BEA.Api
{

    [DataContract]
    public class BEAParamValueItem
    {
        [DataMember]
        public string Key
        {
            get;
            set;
        }

        [DataMember]
        public string Desc
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Key + " = " + Desc;
        }
    }

    [DataContract]
    public class BEAGetParameterValues : BEAResultsBase
    {
        public BEAGetParameterValues()
        {
            ParamValue = new List<BEAParamValueItem>();
        }

        [DataMember]
        public List<BEAParamValueItem> ParamValue
        {
            get;
            set;
        }
    }
}
