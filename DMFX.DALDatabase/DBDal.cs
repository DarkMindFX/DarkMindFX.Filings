using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.DALDatabase
{
    [Export(typeof(IDal))]
    public class DBDal : IDal, IDisposable
    {
        SqlConnection _conn = null;
        object _lockConn = new object();
        private static string schema = "[dbo]";

        public IDalParams CreateDalParams()
        {
            return new DBDalParams();
        }

        public void Dispose()
        {
            if (_conn != null && _conn.State != System.Data.ConnectionState.Closed && _conn.State != System.Data.ConnectionState.Broken)
            {
                lock (_lockConn)
                {
                    if (_conn != null)
                    {
                        _conn.Close();
                        _conn = null;
                    }
                }

            }
        }

        public void Init(IDalParams dalParams)
        {
            if (_conn == null)
            {
                lock (_lockConn)
                {
                    if (_conn == null)
                    {
                        _conn = new SqlConnection(dalParams.Parameters["ConnectionString"]);
                        _conn.Open();
                    }
                }
            }
        }

        #region IDal implementation

        public GetCompanyFilingsInfoResult GetCompanyFilingsInfo(GetCompanyFilingsInfoParams infoParams)
        {
            string spName = "[SP_Get_Company_Filings_Info]";

            GetCompanyFilingsInfoResult result = new GetCompanyFilingsInfoResult();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            // Company code
            SqlParameter paramCompanyCode = new SqlParameter("@company_code", SqlDbType.NVarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, infoParams.CompanyCode);

            // Regulator code
            SqlParameter paramRegulatorCode = new SqlParameter("@regulator_code", SqlDbType.NVarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, infoParams.RegulatorCode);

            // Period start
            SqlParameter paramPeriodStart = new SqlParameter("@start_dt", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, infoParams.PeriodStart);

            // Period end
            SqlParameter paramPeriodEnd = new SqlParameter("@end_dt", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, infoParams.PeriodEnd);

            cmd.Parameters.Add(paramCompanyCode);
            cmd.Parameters.Add(paramRegulatorCode);
            cmd.Parameters.Add(paramPeriodStart);
            cmd.Parameters.Add(paramPeriodEnd);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1)
            {
                // first table - company filings info records
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (infoParams.Types.Count == 0 || infoParams.Types.Contains(row["Type"].ToString()))
                    {
                        CompanyFilingInfo filingInfo = new CompanyFilingInfo();
                        filingInfo.Name = row["Regulator_Filing_Name"].ToString();
                        filingInfo.Type = row["Filing_Type_Code"].ToString();
                        filingInfo.Submitted = (DateTime)row["Filing_Submit_Dt"];
                        filingInfo.PeriodStart = (DateTime)row["Filing_Start_Dt"];
                        filingInfo.PeriodEnd = (DateTime)row["Filing_End_Dt"];

                        result.Filings.Add(filingInfo);
                    }
                }
            }

            result.RegulatorCode = infoParams.RegulatorCode;
            result.CompanyCode = infoParams.CompanyCode;

            return result;
        }

        public GetCompanyFilingResult GetCompanyFilingData(GetCompanyFilingParams cmpFilingParams)
        {
            string spName = "[SP_Get_Company_Filing_Data]";

            GetCompanyFilingResult result = new GetCompanyFilingResult();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            // Company code
            SqlParameter paramCompanyCode = new SqlParameter("@Company_Code", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, cmpFilingParams.CompanyCode);

            // Regulator code
            SqlParameter paramRegulatorCode = new SqlParameter("@regulator_code", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, cmpFilingParams.RegulatorCode);

            // Filing name
            SqlParameter paramFilingName = new SqlParameter("@filing_name", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, cmpFilingParams.Name);

            cmd.Parameters.Add(paramCompanyCode);
            cmd.Parameters.Add(paramRegulatorCode);
            cmd.Parameters.Add(paramFilingName);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds); 

            if (ds.Tables.Count >= 2)
            {
                // first table - metadata
                CompanyFilingInfo filingInfo = new CompanyFilingInfo();
                filingInfo.Name = (string)ds.Tables[0].Rows[0]["Filing_Name"];
                filingInfo.Type = (string)ds.Tables[0].Rows[0]["Type_Code"];
                filingInfo.PeriodEnd = (DateTime)ds.Tables[0].Rows[0]["End_Dt"];
                filingInfo.PeriodStart = (DateTime)ds.Tables[0].Rows[0]["Start_Dt"];
                filingInfo.Submitted = (DateTime)ds.Tables[0].Rows[0]["Submit_Dt"];
                result.FilingInfo = filingInfo;

                // second table - filing data
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    DateTime periodStart = (DateTime)r["Start_Dttm"];
                    DateTime periodEnd = (DateTime)r["End_Dttm"];

                    FilingRecord fr = new FilingRecord();
                    fr.Code = (string)r["Value_Label"];
                    fr.Instant = periodStart == periodEnd ? periodStart : DateTime.MinValue;
                    fr.PeriodEnd = periodEnd;
                    fr.PeriodStart = periodStart;
                    fr.Unit = (string)r["Unit"];
                    fr.Value = (decimal)r["Value"];

                    result.Data.Add(fr);
                }
            }

            return result;
        }

        public void InsertFilingDetails(InsertFilingDetailsParams filingDetails)
        {
            string spName = "[SP_Insert_Filing_Details]";
            // TODO: add SP call  

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            DataTable dtMetadata = ConverToMetadataTable(filingDetails.Metadata);
            DataTable dtFilingData = ConvertToFilingDataTable(filingDetails.Data);

            // Metadata
            SqlParameter paramMetadata = new SqlParameter("@LFM", SqlDbType.Structured);
            paramMetadata.Value = dtMetadata;
            paramMetadata.TypeName = "TYPE_Load_Filing_Meta";
            paramMetadata.Direction = ParameterDirection.Input;

            // FilingData
            SqlParameter paramData = new SqlParameter("@LFD", SqlDbType.Structured);
            paramData.Value = dtFilingData;
            paramData.TypeName = "TYPE_Load_Filing_Details";
            paramData.Direction = ParameterDirection.Input;

            cmd.Parameters.Add(paramMetadata);
            cmd.Parameters.Add(paramData);

            cmd.ExecuteNonQuery();
        }

        public void CreateUserAccount(CreateUserAccountParams createAccountParams)
        {
            string spName = "[SP_Create_User_Account]";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            // User name
            SqlParameter paramName = new SqlParameter("@Name", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, createAccountParams.Name);

            // User email
            SqlParameter paramEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, createAccountParams.Email);

            // User pwd hash
            SqlParameter paramPwdHash = new SqlParameter("@PwdHash", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, createAccountParams.PwdHash);

            // User pwd hash
            SqlParameter paramAccountKey = new SqlParameter("@AccountKey", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, createAccountParams.AccountKey);

            cmd.Parameters.Add(paramName);
            cmd.Parameters.Add(paramEmail);
            cmd.Parameters.Add(paramPwdHash);
            cmd.Parameters.Add(paramAccountKey);

            // TODO: uncomment when SP ready
            //cmd.ExecuteNonQuery();

        }

        public GetUserAccountInfoResult GetUserAccountInfo(GetUserAccountInfoParams accParams)
        {
            string spName = "[SP_Get_User_Account_Info]";
            GetUserAccountInfoResult result = new GetUserAccountInfoResult();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;
            
            // User email
            SqlParameter paramEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, accParams.Email);

            // User pwd hash
            SqlParameter paramAccountKey = new SqlParameter("@AccountKey", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, accParams.AccountKey);

            cmd.Parameters.Add(paramEmail);
            cmd.Parameters.Add(paramAccountKey);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            // TODO: this must be a call to real procedure
            // da.Fill(ds); - UNCOMMENT when SP is ready
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result.AccountKey = (string)ds.Tables[0].Rows[0]["AccountKey"];
                result.Name = (string)ds.Tables[0].Rows[0]["Name"];
                result.Email = (string)ds.Tables[0].Rows[0]["Email"];
                result.PwdHash = (string)ds.Tables[0].Rows[0]["PwdHash"];
                result.DateCreated = (DateTime)ds.Tables[0].Rows[0]["DateCreated"];
            }
            else
            {
                result = null;
            }

            return result;
        }

        public void InitSession(SessionInfo sessionParams)
        {
            string spName = "[SP_Init_Session]";

            GetUserAccountInfoResult result = new GetUserAccountInfoResult();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            // Account key
            SqlParameter paramAccountKey = new SqlParameter("@AccountKey", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, sessionParams.AccountKey);

            // Session id
            SqlParameter paramSessionId = new SqlParameter("@SessionId", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, sessionParams.SessionId);

            // Session id
            SqlParameter paramSessionStart = new SqlParameter("@SessionStart", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, sessionParams.SessionStart);

            cmd.Parameters.Add(paramAccountKey);
            cmd.Parameters.Add(paramSessionId);
            cmd.Parameters.Add(paramSessionStart);

            // TODO: uncomment when SP is ready
            //cmd.ExecuteNonQuery();

        }

        public void CloseSession(SessionInfo sessionParams)
        {
            string spName = "[SP_Close_Session]";

            GetUserAccountInfoResult result = new GetUserAccountInfoResult();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            
            // Session id
            SqlParameter paramSessionId = new SqlParameter("@SessionId", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, sessionParams.SessionId);

            // Session id
            SqlParameter paramSessionStart = new SqlParameter("@SessionEnd", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, sessionParams.SessionEnd);

            cmd.Parameters.Add(paramSessionId);
            cmd.Parameters.Add(paramSessionStart);

            // TODO: uncomment when SP is ready
            //cmd.ExecuteNonQuery();
        }

        public SessionInfo GetSessionInfo(SessionInfo sessionParams)
        {
            string spName = "[SP_Get_User_Account_Info]";

            SessionInfo result = new SessionInfo();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            // User email
            SqlParameter paramEmail = new SqlParameter("@SessionId", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, sessionParams.SessionId);

            cmd.Parameters.Add(paramEmail);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            // TODO: this must be a call to real procedure
            // da.Fill(ds); - UNCOMMENT when SP is ready
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result.AccountKey = (string)ds.Tables[0].Rows[0]["AccountKey"];
                result.SessionId = (string)ds.Tables[0].Rows[0]["SessionId"];
                result.SessionStart = (DateTime)ds.Tables[0].Rows[0]["SessionStart"];
                result.SessionStart = !Convert.IsDBNull(ds.Tables[0].Rows[0]["SessionStart"]) ? (DateTime)ds.Tables[0].Rows[0]["SessionStart"] : DateTime.MinValue;
            }
            else
            {
                result = null;
            }

            return result;
        }

        public GetRegulatorsResult GetRegulators()
        {
            string spName = "[SP_Get_Regulators]";

            GetRegulatorsResult result = new GetRegulatorsResult();
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

   
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;


            da.Fill(ds); 
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    Interfaces.DAL.RegulatorInfo rInfo = new Interfaces.DAL.RegulatorInfo();
                    rInfo.Code = (string)r["Code"];
                    rInfo.Name = System.DBNull.Value != r["Name"] ? (string)r["Name"] : string.Empty;
                    rInfo.CountryCode = (string)r["CountryCode"];

                    result.Regulators.Add(rInfo);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public GetRegulatorCompaniesResult GetCompaniesByRegulator(GetRegulatorCompaniesParams cmpParams)
        {
            string spName = "[SP_Get_Companies_By_Regulator]";

            GetRegulatorCompaniesResult result = new GetRegulatorCompaniesResult();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = _conn;

            // User email
            SqlParameter paramRegCode = new SqlParameter("@RegulatorCode", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, cmpParams.RegulatorCode);

            cmd.Parameters.Add(paramRegCode);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Interfaces.DAL.CompanyInfo cmpInfo = new Interfaces.DAL.CompanyInfo();
                    cmpInfo.LastFilingInfo = new CompanyFilingInfo();

                    cmpInfo.Code = (string)r["Company_Code"];
                    cmpInfo.Name = DBNull.Value != r["Company_Name"] ? (string)r["Company_Name"] : string.Empty;
                    cmpInfo.CompanyRegulatorID = string.Empty; // TODO: need to add extra record to SP result
                    cmpInfo.LastFilingInfo.Name = DBNull.Value != r["Last_Report_Name"] ? (string)r["Last_Report_Name"] : string.Empty;
                    cmpInfo.LastFilingInfo.Type = DBNull.Value != r["Last_Report_Type"] ? (string)r["Last_Report_Type"] : string.Empty;
                    cmpInfo.LastFilingInfo.PeriodEnd = DBNull.Value != r["Last_Report_End_Dt"] ? (DateTime)r["Last_Report_End_Dt"] : DateTime.MinValue;
                    cmpInfo.LastFilingInfo.PeriodStart = DBNull.Value != r["Last_Report_Start_Dt"] ? (DateTime)r["Last_Report_Start_Dt"] : DateTime.MinValue;
                    cmpInfo.LastFilingInfo.Submitted = DBNull.Value != r["Last_Report_Submit_Dt"] ? (DateTime)r["Last_Report_Submit_Dt"] : DateTime.MinValue;

                    result.Companies.Add(cmpInfo);
                }              
            }
            else
            {
                result = null;
            }

            return result;
        }

        #endregion

        #region Support method

        private DataTable ConverToMetadataTable(List<InsertFilingDetailsParams.FilingMetadaRecord> records)
        {
            DataTable dtMetadata = DataAccessTypes.CreateFilingMetadataTable();

            foreach (var r in records)
            {
                DataRow rowMetadata = dtMetadata.NewRow();

                rowMetadata["Name"] = r.Name;
                rowMetadata["Value"] = r.Value;
                rowMetadata["DataType"] = r.Type;

                dtMetadata.Rows.Add(rowMetadata);
            }

            return dtMetadata;


        }

        private DataTable ConvertToFilingDataTable(List<FilingRecord> records)
        {
            DataTable dtFilingData = DataAccessTypes.CreateFilingDataTable();

            foreach (var r in records)
            {
                DataRow rowFilingData = dtFilingData.NewRow();

                rowFilingData["Code"] = r.Code;
                rowFilingData["Value"] = r.Value;
                rowFilingData["UnitName"] = r.Unit;
                rowFilingData["PeriodStart"] = r.PeriodStart != DateTime.MinValue ? r.PeriodStart : r.Instant;
                rowFilingData["PeriodEnd"] = r.PeriodEnd != DateTime.MinValue ? r.PeriodEnd : r.Instant;
                rowFilingData["SourceFactId"] = r.SourceFactId;

                dtFilingData.Rows.Add(rowFilingData);
            }

            return dtFilingData;
        }

        #endregion
    }
}
