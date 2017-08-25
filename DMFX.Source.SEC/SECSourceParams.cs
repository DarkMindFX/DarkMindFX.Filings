using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Source.SEC
{
    public class SECSourceItemInfo : ISourceItemInfo
    {
        public string Name
        {
            get;
            set;
        }
    }

    public class SECSourceSubmissionsInfoParams : ISourceSubmissionsInfoParams
    {
        public SECSourceSubmissionsInfoParams()
        {
            Items = new List<ISourceItemInfo>();
        }
        public string CompanyCode
        {
            get;
            set;
        }

        public List<ISourceItemInfo> Items
        {
            get;
            set;
        }

        public string RegulatorCode
        {
            get;
            set;
        }
    }

    public class SECSourceSubmissionInfo : ISourceSubmissionInfo
    {
        public string Name
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public string Report
        {
            get;
            set;
        }

        public DateTime Submitted
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
    }

    public class SECSourceSubmissionsInfoResult : ResultBase, ISourceSubmissionsInfoResult
    {
        public SECSourceSubmissionsInfoResult()
        {
            Submissions = new List<ISourceSubmissionInfo>();
        }
        public List<ISourceSubmissionInfo> Submissions
        {
            get;
            set;
        }
    }

    public class SECSourceItem : ISourceItem
    {
        public string CompanyCode
        {
            get;
            set;
        }

        public List<byte> Content
        {
            get;
            set;
        }

        public string FilingName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string RegulatorCode
        {
            get;
            set;
        }
    }

    public class SECSourceValidateParams : ISourceValidateParams
    {
        public string CompanyCode
        {
            get;
            set;
        }

        public string RegulatorCode
        {
            get;
            set;
        }
    }

    public class SECSourceValidateResult : ResultBase, ISourceValidateResult
    {
        public SECSourceValidateResult()
        {
            Delta = new List<ISourceItemInfo>();
        }

        public List<ISourceItemInfo> Delta
        {
            get;
            set;
        }

        public bool NeedUpdate
        {
            get;
            set;
        }
    }

    public class SECSourceExtractParams : ISourceExtractParams
    {
        public SECSourceExtractParams()
        {
            Items = new List<ISourceItemInfo>();
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public List<ISourceItemInfo> Items
        {
            get;
            set;
        }

        public string RegulatorCode
        {
            get;
            set;
        }
    }

    public class SECSourceExtractResult : ResultBase, ISourceExtractResult
    {
        public SECSourceExtractResult()
        {
            Items = new List<ISourceItem>();
        }

        public List<ISourceItem> Items
        {
            get;
            set;
        }
    }
}
