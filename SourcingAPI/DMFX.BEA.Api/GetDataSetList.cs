using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.BEA.Api
{
    [DataContract]
    public class BEADataSetItem
    {
        [DataMember]
        public string DatasetName
        {
            get;
            set;
        }

        [DataMember]
        public string DatasetDescription
        {
            get;
            set;
        }

        public override string ToString()
        {
            return "DatasetName = " + DatasetName + "; DatasetDescription = " + DatasetDescription;
        }
    }

    [DataContract]
    public class BEAGetDataSetList : BEAResultsBase
    {
        public BEAGetDataSetList()
        {
            Dataset = new List<BEADataSetItem>();
        }

        [DataMember]
        public List<BEADataSetItem> Dataset
        {
            get;
            set;
        }
    }
}
