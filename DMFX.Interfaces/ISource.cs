using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public interface ISourceItemInfo
    {
        string Name
        {
            get;set;

        }
    }

    public interface ISourceSubmissionInfo
    {
        string Type
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        string Report
        {
            get;
            set;
        }

        DateTime PeriodEnd
        {
            get;
            set;
        }

        DateTime Submitted
        {
            get;
            set;
        }
    }

    public interface ISourceItem
    {
        string RegulatorCode
        {
            get;
            set;
        }

        string CompanyCode
        {
            get;
            set;
        }

        string Name
        {
            get; set;
        }

        List<byte> Content
        {
            get;
            set;
        }

        string FilingName
        {
            get;
            set;
        }
    }

    public interface ISourceValidateParams
    {
        string RegulatorCode
        {
            get;
            set;
        }

        string CompanyCode
        {
            get;
            set;
        }
  
    }

    public interface ISourceValidateResult : IResult
    {
        bool NeedUpdate
        {
            get;
            set;
        }

        List<ISourceItemInfo> Delta
        {
            get;
            set;
        }

        
    }

    public interface ISourceExtractParams
    {
        string RegulatorCode
        {
            get;
            set;
        }

        string CompanyCode
        {
            get;
            set;
        }

        List<ISourceItemInfo> Items
        {
            get;
            set;
        }
    }

    public interface ISourceExtractResult : IResult
    {
        List<ISourceItem> Items
        {
            get;
            set;
        }
    }

    public interface ISourceSubmissionsInfoParams
    {
        string RegulatorCode
        {
            get;
            set;
        }

        string CompanyCode
        {
            get;
            set;
        }

        List<ISourceItemInfo> Items
        {
            get;
            set;
        }
    }

    public interface ISourceSubmissionsInfoResult : IResult
    {
        List<ISourceSubmissionInfo> Submissions
        {
            get;
            set;
        }
    }

    public interface ISource
    {
        ISourceValidateParams CreateValidateParams();
        /// <summary>
        /// Method to validate whether there are any new filings exist for given regulator/company
        /// </summary>
        /// <param name="vldParams"></param>
        /// <returns></returns>
        ISourceValidateResult ValidateSourceDelta(ISourceValidateParams vldParams);

        ISourceExtractParams CreateExtractParams();

        ISourceItemInfo CreateSourceItemInfo();

        ISourceSubmissionsInfoParams CreateSourceSubmissionsInfoParams();

        /// <summary>
        /// Extracts content of the given filings for given regulator/company
        /// </summary>
        /// <param name="extractParams"></param>
        /// <returns></returns>
        ISourceExtractResult ExtractReports(ISourceExtractParams extractParams);

        /// <summary>
        /// Returns information about given submission
        /// </summary>
        /// <param name="infoParams"></param>
        /// <returns></returns>
        ISourceSubmissionsInfoResult GetSubmissionsInfo(ISourceSubmissionsInfoParams infoParams);
    }
}
