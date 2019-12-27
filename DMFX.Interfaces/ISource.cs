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

        IList<string> Report
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

        DateTime UpdateFromDate
        {
            get;
            set;
        }

        DateTime UpdateToDate
        {
            get;
            set;
        }

    }

    public interface ISourceFilingsListResult : IResult
    { 

        List<ISourceItemInfo> Filings
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

    public interface ISourceExtractFilingItemsParams
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

        ISourceItemInfo Filing
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

    public interface ISourceInitParams
    {
        IDictionary Dictionary
        {
            get;
            set;
        }

        IStorage Storage
        {
            get;
            set;
        }

        ILogger Logger
        {
            get;
            set;
        }

        bool ExtractFromStorage
        {
            get;
            set;
        }
    }

    public interface ISource
    {
        /// <summary>
        /// Method to set initial state and to configure with external objects
        /// </summary>
        /// <param name="initParams"></param>
        void Init(ISourceInitParams initParams);

        /// <summary>
        /// Method to get the list of all filings for given regulator/company for given date range
        /// </summary>
        /// <param name="vldParams"></param>
        /// <returns></returns>
        ISourceFilingsListResult GetFilingsList(ISourceValidateParams vldParangsms);
       
        /// <summary>
        /// Extracts content of the given filings for given regulator/company
        /// </summary>
        /// <param name="extractParams"></param>
        /// <returns></returns>
        ISourceExtractResult ExtractReports(ISourceExtractParams extractParams);

        /// <summary>
        /// Extracts single item with given name from the filing
        /// </summary>
        /// <param name="extractItemParams"></param>
        /// <returns></returns>
        ISourceExtractResult ExtractFilingItems(ISourceExtractFilingItemsParams extractItemsParams);

        /// <summary>
        /// Returns information about given submission
        /// </summary>
        /// <param name="infoParams"></param>
        /// <returns></returns>
        ISourceSubmissionsInfoResult GetSubmissionsInfo(ISourceSubmissionsInfoParams infoParams);

        ISourceInitParams CreateInitParams();
        ISourceExtractParams CreateExtractParams();
        ISourceItemInfo CreateSourceItemInfo();
        ISourceValidateParams CreateValidateParams();
        ISourceSubmissionsInfoParams CreateSourceSubmissionsInfoParams();
        ISourceExtractFilingItemsParams CreateSourceExtractFilingItemsParams();
    }
}
