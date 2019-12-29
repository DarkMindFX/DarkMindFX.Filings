using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces.DAL
{
    public interface IDalParams
    {
        Dictionary<string, string> Parameters
        {
            get;
        }
    }

    #region Params / results strcutures

    public struct FilingRecord
    {
        public string Code
        {
            get;
            set;
        }

        public decimal? Value
        {
            get;
            set;
        }

        public string Value_Str
        {
            get;
            set;
        }

        public DateTime? Value_Dttm
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public DateTime Instant
        {
            get;
            set;
        }

        public string Unit
        {
            get;
            set;
        }

        public string SourceFactId
        {
            get;
            set;
        }

        public string FactId
        {
            get;
            set;
        }
    }

    public class InsertFilingDetailsParams
    {
        public struct FilingMetadaRecord
        {
            public string Name
            {
                get;
                set;
            }

            public string Value
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


        public InsertFilingDetailsParams()
        {
            Metadata = new List<FilingMetadaRecord>();
            Data = new List<FilingRecord>();
        }

        public List<FilingMetadaRecord> Metadata
        {
            get;
            set;
        }

        public List<FilingRecord> Data
        {
            get;
            set;
        }

    }

    public class InsertFilingDetailsResult
    {
        public long FilingId
        {
            get;
            set;
        }
    }

    public class GetCompanyFilingsInfoParams
    {
        public GetCompanyFilingsInfoParams()
        {
            Types = new HashSet<string>();
            PeriodStart = DateTime.Parse("1994-01-01");
            PeriodEnd = DateTime.UtcNow;
        }

        public string RegulatorCode
        {
            get;
            set;
        }
        public string CompanyCode
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public HashSet<string> Types
        {
            get;
            set;
        }
    }

    public class CompanyFilingInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public DateTime Submitted
        {
            get;
            set;
        }
    }

    public class GetCompanyFilingsInfoResult
    {
        public GetCompanyFilingsInfoResult()
        {
            Filings = new List<CompanyFilingInfo>();
        }
        public string RegulatorCode
        {
            get;
            set;
        }
        public string CompanyCode
        {
            get;
            set;
        }

        public List<CompanyFilingInfo> Filings
        {
            get;
            set;
        }
    }

    public class GetCompanyFilingParams
    {
        public GetCompanyFilingParams(string[] valueCodes = null)
        {
            Values = new HashSet<string>();

            if (valueCodes != null)
            {
                foreach (var code in valueCodes)
                {
                    if (!Values.Contains(code))
                    {
                        Values.Add(code);
                    }
                }
            }
        }
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
        public string Name
        {
            get;
            set;
        }

        public HashSet<string> Values
        {
            get;
            set;
        }
    }

    public class GetCompanyFilingResult
    {
        public GetCompanyFilingResult()
        {
            Data = new List<FilingRecord>();
        }

        public CompanyFilingInfo FilingInfo
        {
            get;
            set;
        }

        public List<FilingRecord> Data
        {
            get;
            set;
        }
    }

    public class GetCompanyFilingRatiosParams
    {
        public GetCompanyFilingRatiosParams(string[] valueCodes = null)
        {
            Values = new HashSet<string>();

            if (valueCodes != null)
            {
                foreach (var code in valueCodes)
                {
                    if (!Values.Contains(code))
                    {
                        Values.Add(code);
                    }
                }
            }
        }
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
        public string Name
        {
            get;
            set;
        }

        public HashSet<string> Values
        {
            get;
            set;
        }
    }

    public class GetCompanyFilingRatiosResult
    {
        public GetCompanyFilingRatiosResult()
        {
            Data = new List<FilingRecord>();
        }
        public CompanyFilingInfo FilingInfo
        {
            get;
            set;
        }

        public List<FilingRecord> Data
        {
            get;
            set;
        }
    }

    public class GetCommonSizeBalanceSheetParams
    {
        public GetCommonSizeBalanceSheetParams(string[] valueCodes = null)
        {
            Values = new HashSet<string>();

            if (valueCodes != null)
            {
                foreach (var code in valueCodes)
                {
                    if (!Values.Contains(code))
                    {
                        Values.Add(code);
                    }
                }
            }
        }
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
        public string Name
        {
            get;
            set;
        }

        public HashSet<string> Values
        {
            get;
            set;
        }
    }

    public class GetCommonSizeBalanceSheetResult
    {
        public GetCommonSizeBalanceSheetResult()
        {
            Data = new List<FilingRecord>();
        }
        public CompanyFilingInfo FilingInfo
        {
            get;
            set;
        }

        public List<FilingRecord> Data
        {
            get;
            set;
        }
    }

    public class GetCommonSizeIncomeStatementParams
    {
        public GetCommonSizeIncomeStatementParams(string[] valueCodes = null)
        {
            Values = new HashSet<string>();

            if (valueCodes != null)
            {
                foreach (var code in valueCodes)
                {
                    if (!Values.Contains(code))
                    {
                        Values.Add(code);
                    }
                }
            }
        }
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
        public string Name
        {
            get;
            set;
        }

        public HashSet<string> Values
        {
            get;
            set;
        }
    }

    public class GetCommonSizeIncomeStatementResult
    {
        public GetCommonSizeIncomeStatementResult()
        {
            Data = new List<FilingRecord>();
        }
        public CompanyFilingInfo FilingInfo
        {
            get;
            set;
        }

        public List<FilingRecord> Data
        {
            get;
            set;
        }
    }

    public class GetUserAccountInfoParams
    {
        public string Email
        {
            get;
            set;
        }
        public string AccountKey
        {
            get;
            set;
        }
    }

    public class GetUserAccountInfoResult : ResultBase
    {
        public Int64 UserId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string AccountKey
        {
            get;
            set;
        }

        public string PwdHash
        {
            get;
            set;
        }

        public DateTime DateCreated
        {
            get;
            set;
        }

        public DateTime DateExpires
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string ActivationCode
        {
            get;
            set;
        }
    }

    public class CreateUpdateUserAccountParams
    {
        public int UserId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string PwdHash
        {
            get;
            set;
        }

        public string AccountKey
        {
            get;
            set;
        }

        public string ActivationCode
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }
    }

    public class SessionInfo
    {
        public string AccountKey
        {
            get;
            set;
        }

        public string SessionId
        {
            get;
            set;
        }

        public DateTime SessionStart
        {
            get;
            set;
        }

        public DateTime SessionEnd
        {
            get;
            set;
        }

        public DateTime SessionExpires
        {
            get;
            set;
        }
    }

    public class RegulatorInfo
    {
        public string Code
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }
    }

    public class GetRegulatorsResult
    {
        public GetRegulatorsResult()
        {
            Regulators = new List<RegulatorInfo>();
        }
        public List<RegulatorInfo> Regulators
        {
            get;
            set;
        }
    }

    public class CompanyInfo
    {
        public string Code
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string CompanyRegulatorID
        {
            get;
            set;
        }

        public DateTime LastUpdated
        {
            get;
            set;
        }

        public CompanyFilingInfo LastFilingInfo
        {
            get;
            set;
        }
    }

    public class GetRegulatorCompaniesParams
    {
        public string RegulatorCode
        {
            get;
            set;
        }
    }

    public class GetRegulatorCompaniesResult
    {
        public GetRegulatorCompaniesResult()
        {
            Companies = new List<CompanyInfo>();
        }

        public List<CompanyInfo> Companies
        {
            get;
            set;
        }

    }

    public class ProcessFilingParams
    {
        public long FilingId
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public IDictionary<string, string> Parameters
        {
            get;
            set;
        }
    }

    public class ProcessFilingResult
    {
        public long FilingId
        {
            get;
            set;
        } 
    }

    #endregion


    public interface IDal
    {
        void Init(IDalParams dalParams);

        IDalParams CreateDalParams();

        /// <summary>
        /// Inserts filing data into storage
        /// </summary>
        /// <param name="filingDetails"></param>
        InsertFilingDetailsResult InsertFilingDetails(InsertFilingDetailsParams filingDetails);

        /// <summary>
        /// Returns list of available filings for the given company
        /// </summary>
        /// <param name="infoParams"></param>
        /// <returns></returns>
        GetCompanyFilingsInfoResult GetCompanyFilingsInfo(GetCompanyFilingsInfoParams infoParams);

        /// <summary>
        /// Returns details of the company's specific filing
        /// </summary>
        /// <param name="cmpFilingParams"></param>
        /// <returns></returns>
        GetCompanyFilingResult GetCompanyFilingData(GetCompanyFilingParams cmpFilingParams);

        /// <summary>
        /// For the given filing of the given company returns set of calculated financial ratios
        /// </summary>
        /// <param name="cmlFilingRatiosParams"></param>
        /// <returns></returns>
        GetCompanyFilingRatiosResult GetCompanyFilingRatios(GetCompanyFilingRatiosParams cmlFilingRatiosParams);

        /// <summary>
        /// For the given filing of the given company returns common size balance sheet
        /// </summary>
        /// <param name="cmpCommonSizeBSParams"></param>
        /// <returns></returns>
        GetCommonSizeBalanceSheetResult GetCommonSizeBalanceSheet(GetCommonSizeBalanceSheetParams cmpCommonSizeBSParams);

        /// <summary>
        /// For the given filing of the given company returns common size income statement
        /// </summary>
        /// <param name="cmpCommonSizeISParams"></param>
        /// <returns></returns>
        GetCommonSizeIncomeStatementResult GetCommonSizeIncomeStatement(GetCommonSizeIncomeStatementParams cmpCommonSizeISParams);

        /// <summary>
        /// Create user accont record
        /// </summary>
        /// <param name="createAccountParams"></param>
        void CreateUserAccount(CreateUpdateUserAccountParams createAccountParams);

        /// <summary>
        /// Update user accont record
        /// </summary>
        /// <param name="updateAccountParams"></param>
        void UpdateUserAccount(CreateUpdateUserAccountParams updateAccountParams);

        /// <summary>
        /// Returns user account details
        /// </summary>
        /// <param name="accParams"></param>
        /// <returns></returns>
        GetUserAccountInfoResult GetUserAccountInfo(GetUserAccountInfoParams accParams);

        /// <summary>
        /// Adds new session record
        /// </summary>
        /// <param name="sessionParams"></param>
        void InitSession(SessionInfo sessionParams);

        /// <summary>
        /// Marks session as closed
        /// </summary>
        /// <param name="sessionParams"></param>
        void CloseSession(SessionInfo sessionParams);

        /// <summary>
        /// Extracts and returns information about the session with specific id
        /// </summary>
        /// <param name="sessionParams"></param>
        /// <returns></returns>
        SessionInfo GetSessionInfo(SessionInfo sessionParams, bool checkActive);

        /// <summary>
        /// Returns list of all available regulators 
        /// </summary>
        /// <returns></returns>
        GetRegulatorsResult GetRegulators();

        /// <summary>
        /// Returns list of companies who file report to the given regulator
        /// </summary>
        /// <param name="cmpParams"></param>
        /// <returns></returns>
        GetRegulatorCompaniesResult GetCompaniesByRegulator(GetRegulatorCompaniesParams cmpParams);

        /// <summary>
        /// Executes custom functions to process given filing based on the parameters provided
        /// </summary>
        /// <param name="prcFilingsParams"></param>
        /// <returns></returns>
        ProcessFilingResult ProcessFiling(ProcessFilingParams prcFilingsParams);





    }
}
