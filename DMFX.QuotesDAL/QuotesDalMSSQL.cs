using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesDAL
{
    [Export("MSSQL", typeof(IQuotesDal))]
    public class QuotesDalMSSQL : IQuotesDal
    {
        private object _lockConn = new object();
        private static string schema = "[dbo]";
        IQuotesDalInitParams _dalParams = null;
        public void Init(IQuotesDalInitParams initParams)
        {
            _dalParams = initParams;
        }

        public IQuotesDalGetTimeseriesValuesResult GetTimseriesValues(IQuotesDalGetTimeSeriesValuesParams getQuotesParams)
        {
            IQuotesDalGetTimeseriesValuesResult result = new QuotesDalMSSQLGetQuotesResult();

            string spName = "[SP_Get_Ticker_Timeseries_Values]";
            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");


            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            SqlParameter paramTypeId = new SqlParameter("@IN_Period_Type_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, (int)getQuotesParams.TimeFrame);

            SqlParameter paramTickerId = new SqlParameter("@IN_Ticker_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, 0);

            cmd.Parameters.Add(paramTypeId);
            cmd.Parameters.Add(paramTickerId);

            for (int i = 0; i < getQuotesParams.Tickers.Count; ++i)
            {
                // getting ID of the current ticker
                paramTickerId.Value = GetTickerId(getQuotesParams.Tickers[i], conn);

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                try
                {

                    da.Fill(ds);

                    if (ds.Tables.Count >= 1)
                    {

                        BaseQuotesData qdata = new BaseQuotesData();
                        qdata.Ticker = getQuotesParams.Tickers[i];
                        qdata.Country = getQuotesParams.Country;
                        qdata.TimeFrame = getQuotesParams.TimeFrame;

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            DateTime dtEvent = (DateTime)row[0];
                            if (dtEvent >= getQuotesParams.PeriodStart && dtEvent <= getQuotesParams.PeriodEnd)
                            {
                                int valCount = row.ItemArray.Count(x => !DBNull.Value.Equals(x));
                                ITimeSeriesRecord qrec = new CustomTimeseriesRecord(new List<string>(new string[valCount - 1]), dtEvent);

                                for (int r = 1; r <= valCount - 1; ++r)
                                {
                                    qrec[r - 1] = (decimal)row[r];
                                }

                                qdata.AddRecord(qrec);
                            }
                        }

                        result.Quotes.Add(qdata);
                    }

                }
                catch (Exception ex)
                {
                    result.Errors.Add(new Interfaces.Error()
                    {
                        Code = Interfaces.EErrorCodes.QuotesNotFound,
                        Type = Interfaces.EErrorType.Error,
                        Message = string.Format("'{0}, {1}': failed to read timeseries. SQL message: {2}", getQuotesParams.Tickers[i], getQuotesParams.TimeFrame, ex.Message)
                    });
                }
            }

            if (result.Quotes.Count == 0 && getQuotesParams.Tickers.Count != 0)
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
            }

            conn.Close();

            return result;
        }

        public IQuotesDalSaveTimeseriesValuesResult SaveTimeseriesValues(IQuotesDalSaveTimeseriesValuesParams saveQuotesParams)
        {
            IQuotesDalSaveTimeseriesValuesResult result = new QuotesDalMSSQLSaveQuotesResult();

            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            foreach (var q in saveQuotesParams.Quotes)
            {
                if (q.Quotes.Count > 0)
                {
                    // getting ID of the ticker
                    long tickerId = GetTickerId(q.Ticker, conn);
                    if (tickerId == long.MinValue)
                    {
                        tickerId = AddTicker(q.Ticker, q.Name, q.AgencyCode, q.Notes, conn);
                        if (tickerId != Int32.MinValue && q.Metadata != null && q.Metadata.Values.Count > 0)
                        {
                            SetTickerMetadata(tickerId, q.Metadata, conn);
                        }
                    }

                    int unitId = (int)q.Unit;
                    int timeFrameId = (int)q.TimeFrame;
                    int typeId = (int)q.Type;

                    // getting TS info for this ticker
                    long tsId = GetTimeSeriesId(tickerId, timeFrameId, conn);
                    if (tsId == long.MinValue)
                    {
                        // creating new record
                        tsId = CreateTimeseries(tickerId, unitId, typeId, timeFrameId, q.Quotes[0].ValueNames.ToList(), conn);
                    }
                    if (tsId == Int32.MinValue)
                    {
                        result.AddError(
                            Interfaces.EErrorCodes.InvalidSourceParams,
                            Interfaces.EErrorType.Warning,
                            string.Format("Failed to insert {0} - verify input parameters", q.Ticker));
                    }
                    else
                    {
                        // preparing staging table
                        string stageTable = PrepareStageTable(q.Quotes[0].ValueNames.Count, conn);
                        if (!string.IsNullOrEmpty(stageTable))
                        {
                            DataTable dtTimeSeriesValues = ConvertToLoadTimeSeries(q);

                            LoadToStagingTable(q.Quotes[0].ValueNames.Count, stageTable, dtTimeSeriesValues, conn);

                            TriggerStageToCore(stageTable, tsId, conn);

                            result.TimeSeriesSaved.Add(tickerId);

                        }
                        else
                        {
                            result.AddError(
                            Interfaces.EErrorCodes.InvalidSourceParams,
                            Interfaces.EErrorType.Warning,
                            string.Format("Failed to insert {0} - stage table was not created", q.Ticker));
                        }
                    }
                }
            }

            conn.Close();

            result.Success = result.TimeSeriesSaved != null && result.TimeSeriesSaved.Count > 0;

            return result;
        }

        public IQuotesDalGetTickersListResult GetTickersList(IQuotesDalGetTickersListParams getTsList)
        {
            IQuotesDalGetTickersListResult result = new QuotesDalMSSQLGetTickersListResult();

            string spName = "[SP_Get_Tickers_List]";
            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            long agencyId = !string.IsNullOrEmpty(getTsList.Agency) ? GetAgencyId(getTsList.Agency, conn) : Int32.MinValue;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            SqlParameter paramTypeId = new SqlParameter("@IN_Type_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, (int)getTsList.Type);
            cmd.Parameters.Add(paramTypeId);

            if (agencyId > Int32.MinValue)
            {
                SqlParameter paramAgencyId = new SqlParameter("@IN_Agency_Id", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, agencyId);
                cmd.Parameters.Add(paramAgencyId);
            }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    result.Tickers.Add(new TickersListItem()
                    {
                        CountryCode = getTsList.CountryCode,
                        Ticker = (string)r["Ticker_Symbol"],
                        Name = (string)r["Ticker_Name"],
                        Unit = (EUnit)r["TS_Unit_Id"],
                        Type = getTsList.Type
                    });
                }

                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.EmptyCollection,
                    Message = string.Format("Failed to find tickers for {0}, {1}", getTsList.Type, getTsList.CountryCode)
                });
            }

            return result;
        }

        public IQuotesDalGetTimeSeriesInfoResult GetTimeSeriesInfo(IQuotesDalGetTimeSeriesInfoParams getTsInfoParams)
        {
            IQuotesDalGetTimeSeriesInfoResult result = new QuotesDalMSSQLGetTimeSeriesInfoResult();

            string spName = "[SP_GetTimeseries_Info_By_Ticket]";

            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            SqlParameter paramTicker = new SqlParameter("@IN_Ticker", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, getTsInfoParams.Ticker);

            SqlParameter paramCountryCode = new SqlParameter("@IN_Country_Code", SqlDbType.NVarChar, 255, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Current, getTsInfoParams.CountryCode);

            cmd.Parameters.Add(paramTicker);
            cmd.Parameters.Add(paramCountryCode);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            const int cColumnCount = 10; // TODO: currenty data columns fixed to 10 - need to made it flexible and extandeble

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
            {
                // getting data from the first record
                result.CountryCode = getTsInfoParams.CountryCode;
                result.Ticker = getTsInfoParams.Ticker;
                result.Name = (string)ds.Tables[0].Rows[0][0];
                result.Unit = (EUnit)ds.Tables[0].Rows[0][1];
                result.Type = (ETimeSeriesType)ds.Tables[0].Rows[0][2];
                for (int i = 1; i <= cColumnCount && !DBNull.Value.Equals(ds.Tables[0].Rows[0][string.Format("Column_{0}", i)]); ++i)
                {
                    result.Columns.Add((string)ds.Tables[0].Rows[0][string.Format("Column_{0}", i)]);
                }

                // creating list of available timeframes
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    result.Series.Add(new TimeSeriesInfoListItem()
                    {
                        PeriodStart = (DateTime)r["Period_Start"],
                        PeriodEnd = (DateTime)r["Period_End"],
                        Timeframe = (ETimeFrame)r["PeriodTypeName"]

                    });
                }

                // getting metadata - if exists
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    result.Metadata = new Dictionary<string, string>();
                    for (int r = 0; r < ds.Tables[1].Rows.Count; ++r)
                    {
                        result.Metadata.Add((string)ds.Tables[1].Rows[r]["Ticker_Meta_Key"], (string)ds.Tables[1].Rows[r]["Ticker_Meta_Value"]);
                    }
                }

            }
            else
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.TickerNotFound,
                    Type = Interfaces.EErrorType.Error,
                    Message = string.Format("Failed to find info for ticker {0}, countrty {1}", getTsInfoParams.Ticker, getTsInfoParams.CountryCode)
                });
            }

            conn.Close();

            return result;
        }

        public IQuotesDalProcessTickerResult ProcessTicker(IQuotesDalProcessTickerParams procTickerParams)
        {
            IQuotesDalProcessTickerResult result = new QuotesDalMSSQLProcessTickerResult();

            try
            {
                switch (procTickerParams.Type)
                {
                    case "ETL":
                        result = RunTickerETL(procTickerParams.TickerId);
                        break;
                    default:
                        result.Errors.Add(new Interfaces.Error()
                        {
                            Code = Interfaces.EErrorCodes.TickerProcessingFail,
                            Type = Interfaces.EErrorType.Error,
                            Message = string.Format("Unknown processing type '{0}'", procTickerParams.Type)
                        }
                        ); 
                        result.Success = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.TickerProcessingFail,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                }
                );
                result.Success = false;
            }

            return result;

        }


        #region Create* methods

        public IQuotesDalGetTimeSeriesValuesParams CreateGetQuotesParams()
        {
            return new QuotesDalMSSQLGetQuotesParams();
        }

        public IQuotesDalInitParams CreateInitParams()
        {
            return new QuotesDalMSSQLInitParams();
        }

        public IQuotesDalSaveTimeseriesValuesParams CreateSaveTimeseriesValuesParams()
        {

            return new QuotesDalMSSQLSaveQuotesParams();
        }

        public IQuotesDalGetTimeSeriesInfoParams CreateGetTimeSeriesInfoParams()
        {
            return new QuotesDalMSSQLGetTimeSeriesInfoParams();
        }

        public IQuotesDalGetTickersListParams CreateGetTickersListParams()
        {
            return new QuotesDalMSSQLGetTickersListParams();
        }

        public IQuotesDalProcessTickerParams CreateProcessTickerParams()
        {
            return new QuotesDalMSSQLProcessTickerParams();
        }

        #endregion

        #region Support method

        private IQuotesDalProcessTickerResult RunTickerETL(long tickerId)
        {
            IQuotesDalProcessTickerResult result = new QuotesDalMSSQLProcessTickerResult();

            string spName = "[SP_Ticker_ETL]";

            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerId = new SqlParameter("@IN_Ticker_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, tickerId);
            cmd.Parameters.Add(paramTickerId);

            cmd.ExecuteNonQuery();

            conn.Close();

            result.Success = true;

            return result;
        }

        private void TriggerStageToCore(string stageTable, long timeSeriesId, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_STG_2_CORE]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramStageTable = new SqlParameter("@IN_STG_Table_Name", SqlDbType.VarChar, 30, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, stageTable);
            cmd.Parameters.Add(paramStageTable);

            var paramTimeSeriesId = new SqlParameter("@IN_TS_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, timeSeriesId);
            cmd.Parameters.Add(paramTimeSeriesId);

            cmd.ExecuteNonQuery();
        }

        private void LoadToStagingTable(int columnsCount, string stageTable, DataTable data, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Load_Stage]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramColsNumber = new SqlParameter("@IN_Columns_Number", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, columnsCount);
            cmd.Parameters.Add(paramColsNumber);

            var paramStageTable = new SqlParameter("@IN_Stage_Table", SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, stageTable);
            cmd.Parameters.Add(paramStageTable);

            var paramData = new SqlParameter("@IN_Data", SqlDbType.Structured);
            paramData.Value = data;
            paramData.TypeName = "TYPE_Load_TimeSeries";
            paramData.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(paramData);

            cmd.ExecuteNonQuery();


        }

        private DataTable ConvertToLoadTimeSeries(IQuotesData data)
        {
            DataTable dtFilingData = DataAccessTypes.CreateLoadTimeSeriesMetadataTable();

            foreach (var r in data.Quotes)
            {
                DataRow rowFilingData = dtFilingData.NewRow();

                rowFilingData["Even_Date"] = r.Time;
                for (int i = 0; i < r.Values.Count; ++i)
                {
                    rowFilingData[string.Format("Col_{0}", i + 1)] = r.Values[i];
                }


                dtFilingData.Rows.Add(rowFilingData);
            }

            return dtFilingData;
        }

        private DataTable ConvertToTickerMetadata(ITimeSeriesMetadata data)
        {
            DataTable dtFilingData = DataAccessTypes.CreateTickerMetadataTable();

            foreach (var k in data.Values.Keys)
            {
                DataRow rowFilingData = dtFilingData.NewRow();

                rowFilingData["Key"] = k;
                rowFilingData["Value"] = data.Values[k];

                dtFilingData.Rows.Add(rowFilingData);
            }

            return dtFilingData;
        }

        private long GetTickerId(string ticker, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Get_Ticker_Id]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTicker = new SqlParameter("@IN_Ticker_Symbol", SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, ticker);
            cmd.Parameters.Add(paramTicker);

            var paramOutTickerId = new SqlParameter("@OUT_Ticker_Id", SqlDbType.BigInt, 0, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current, null);
            cmd.Parameters.Add(paramOutTickerId);

            cmd.ExecuteNonQuery();

            return !DBNull.Value.Equals(paramOutTickerId.Value) ? (long)paramOutTickerId.Value: long.MinValue;
        }

        private bool SetTickerMetadata(long tickerId, ITimeSeriesMetadata metadata, SqlConnection conn)
        {
            bool result = false;

            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Set_Ticker_Metadata]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerSymbol = new SqlParameter("@IN_Ticker_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, tickerId);
            cmd.Parameters.Add(paramTickerSymbol);

            var paramTickerMeta = new SqlParameter("@IN_Ticker_Metadata", SqlDbType.Structured);
            paramTickerMeta.Value = ConvertToTickerMetadata(metadata);
            paramTickerMeta.TypeName = "TYPE_Ticker_Metadata";
            paramTickerMeta.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(paramTickerMeta);

            cmd.ExecuteNonQuery();

            result = true;

            return result;

        }

        private long AddTicker(string ticker, string name, string agency, string notes, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Add_Ticker_Id]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerSymbol = new SqlParameter("@IN_Ticker_Symbol", SqlDbType.NVarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, ticker);
            cmd.Parameters.Add(paramTickerSymbol);

            var paramTickerName = new SqlParameter("@IN_Ticker_Name", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, !string.IsNullOrEmpty(name) ? name : string.Empty);
            cmd.Parameters.Add(paramTickerName);

            if (!string.IsNullOrEmpty(notes))
            {
                var paramTickerNotes = new SqlParameter("@IN_Ticker_Notes", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, !string.IsNullOrEmpty(notes) ? notes : string.Empty);
                cmd.Parameters.Add(paramTickerNotes);
            }

            if (!string.IsNullOrEmpty(agency))
            {
                var paramAgencyCode = new SqlParameter("@IN_Agency_Code", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, !string.IsNullOrEmpty(agency) ? agency : string.Empty);
                cmd.Parameters.Add(paramAgencyCode);
            }

            var paramOutTickerId = new SqlParameter("@OUT_Ticker_Id", SqlDbType.BigInt, 0, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current, null);
            cmd.Parameters.Add(paramOutTickerId);

            cmd.ExecuteNonQuery();

            return !DBNull.Value.Equals(paramOutTickerId.Value) ? (long)paramOutTickerId.Value : long.MinValue;
        }

        private long GetAgencyId(string agencyCode, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Get_Agency_Id]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramAgencyCode = new SqlParameter("@IN_Agency_Code", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, agencyCode);
            cmd.Parameters.Add(paramAgencyCode);

            var paramOutAgencyId = new SqlParameter("@OUT_Agency_Id", SqlDbType.BigInt, 0, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current, null);
            cmd.Parameters.Add(paramOutAgencyId);

            cmd.ExecuteNonQuery();

            return !DBNull.Value.Equals(paramOutAgencyId.Value) ? (long)paramOutAgencyId.Value : Int32.MinValue;
        }

        private long GetTimeSeriesId(long tickerId, int periodId, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Get_Timeseries_By_Ticker]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerId = new SqlParameter("@IN_Ticker_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, tickerId);

            var paramPeriodId = new SqlParameter("@IN_Period_Type_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, periodId);


            cmd.Parameters.Add(paramTickerId);
            cmd.Parameters.Add(paramPeriodId);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
            {
                return !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]) ? (int)ds.Tables[0].Rows[0][0] : long.MinValue;
            }
            else
            {
                return long.MinValue;
            }
        }

        private long CreateTimeseries(long tickerId, int unitId, int tsTypeId, int timeframeId, List<string> columns, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Create_Timeseries]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerId = new SqlParameter("@IN_Ticker_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, tickerId);
            cmd.Parameters.Add(paramTickerId);

            var paramDesc = new SqlParameter("@IN_TS_Description", SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, string.Empty);
            cmd.Parameters.Add(paramDesc);

            var paramUitId = new SqlParameter("@IN_TS_Unit_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, unitId);
            cmd.Parameters.Add(paramUitId);

            var paramTypeId = new SqlParameter("@IN_TS_Type_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, tsTypeId);
            cmd.Parameters.Add(paramTypeId);

            var paramTimeFrameId = new SqlParameter("@IN_TS_Period_Type_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, timeframeId);
            cmd.Parameters.Add(paramTimeFrameId);

            var paramTSColumnsCount = new SqlParameter("@IN_TS_Columns_Count", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, columns.Count);
            cmd.Parameters.Add(paramTSColumnsCount);

            for (int i = 0; i < columns.Count; ++i)
            {
                var paramColumn = new SqlParameter(string.Format("IN_Column_{0}_Name", i + 1), SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, columns[i]);
                cmd.Parameters.Add(paramColumn);
            }

            var paramOutTSId = new SqlParameter("@OUT_TS_Id", SqlDbType.BigInt, 0, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current, null);
            cmd.Parameters.Add(paramOutTSId);

            cmd.ExecuteNonQuery();

            return !DBNull.Value.Equals(paramOutTSId.Value) ? (long)paramOutTSId.Value : long.MinValue;

        }

        private string PrepareStageTable(int columns, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Prepare_Stage]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerName = new SqlParameter("@IN_Column_Number", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, columns);

            var paramOutTableName = new SqlParameter("@OUT_Table_Name", SqlDbType.NVarChar, 255, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current, null);

            cmd.Parameters.Add(paramTickerName);
            cmd.Parameters.Add(paramOutTableName);

            cmd.ExecuteNonQuery();

            return (string)paramOutTableName.Value;
        }



        private SqlConnection OpenConnection(string name)
        {
            SqlConnection conn = new SqlConnection(_dalParams.Parameters[name]);
            conn.Open();

            return conn;
        }



        #endregion

    }
}
