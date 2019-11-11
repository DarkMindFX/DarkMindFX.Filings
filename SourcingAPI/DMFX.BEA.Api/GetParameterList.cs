using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.BEA.Api
{
    [DataContract]
    public class BEAParameterItem
    {
        [DataMember]
        public string ParameterName
        {
            get;
            set;
        }

        [DataMember]
        public string ParameterDataType
        {
            get;
            set;
        }

        [DataMember]
        public string ParameterDescription
        {
            get;
            set;
        }

        [DataMember]
        public string ParameterDefaultValue
        {
            get;
            set;
        }

        [DataMember]
        public string AllValue
        {
            get;
            set;
        }

        [DataMember]
        public int ParameterIsRequiredFlag
        {
            get;
            set;
        }

        [DataMember]
        public int MultipleAcceptedFlag
        {
            get;
            set;
        }

        public override string ToString()
        {
            return "ParameterName = " + ParameterName;
        }
    }


    [DataContract]
    public class BEAGetParameterList : BEAResultsBase
    {
        public BEAGetParameterList()
        {
            Parameter = new List<BEAParameterItem>();
        }
        public List<BEAParameterItem> Parameter
        {
            get;
            set;
        }
    }
}
