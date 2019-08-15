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

        public IQuotesDalGetQuotesResult GetQuotes(IQuotesDalGetQuotesParams getQuotesParams)
        {
            IQuotesDalGetQuotesResult result = new QuotesDalMSSQLGetQuotesResult();

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
                            BaseQuotesRecord qrec = new BaseQuotesRecord();
                            qrec.Time = (DateTime)row[0];
                            for (int r = 1; r < row.Table.Columns.Count - 1; ++r)
                            {
                                qrec[r-1] = (decimal)row[r];
                            }

                            qdata.AddRecord(qrec);
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

            return result;
        }

        public IQuotesDalSaveQuotesResult SaveQuotes(IQuotesDalSaveQuotesParams saveQuotesParams)
        {
            IQuotesDalSaveQuotesResult result = new QuotesDalMSSQLSaveQuotesResult();

            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            foreach (var q in saveQuotesParams.Quotes)
            {
                // getting ID of the ticker
                int tickerId = GetTickerId(q.Ticker, conn);
                if (tickerId == Int32.MinValue)
                {
                    tickerId = AddTicker(q.Ticker, conn);
                }

                // getting TS info for this ticker
                int tsId = GetTimeSeriesId(tickerId, conn);
                if (tsId == Int32.MinValue)
                {
                    int unitId = (int)q.Unit;
                    int timeFrameId = (int)q.TimeFrame;
                    int typeId = (int)q.Type;

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

                        ++result.TimeSeriesSaved;

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

            result.Success = result.TimeSeriesSaved > 0;

            return result;
        }


        #region Create* methods

        public IQuotesDalGetQuotesParams CreateGetQuotesParams()
        {
            return new QuotesDalMSSQLGetQuotesParams();
        }

        public IQuotesDalInitParams CreateInitParams()
        {
            return new QuotesDalMSSQLInitParams();
        }

        public IQuotesDalSaveQuotesParams CreateSaveQuotesParams()
        {

            return new QuotesDalMSSQLSaveQuotesParams();
        }

        #endregion

        #region Support method

        private void TriggerStageToCore(string stageTable, int timeSeriesId, SqlConnection conn)
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

        private int GetTickerId(string ticker, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Get_Ticker_Id]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerName = new SqlParameter("@IN_Ticker_Name", SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, ticker);
            cmd.Parameters.Add(paramTickerName);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
            {
                return !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]) ? (int)ds.Tables[0].Rows[0][0] : Int32.MinValue;
            }
            else
            {
                return Int32.MinValue;
            }
        }

        private int AddTicker(string ticker, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Add_Ticker_Id]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerName = new SqlParameter("@IN_Ticker_Name", SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, ticker);
            cmd.Parameters.Add(paramTickerName);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
            {
                return !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]) ? (int)ds.Tables[0].Rows[0][0] : Int32.MinValue;
            }
            else
            {
                return Int32.MinValue;
            }
        }

        private int GetTimeSeriesId(int tickerId, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();

            //first checking if TS exists
            string spName = "[SP_Get_Timeseries_By_Ticker]";
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            var paramTickerId = new SqlParameter("@IN_Ticker_Id", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, tickerId);


            cmd.Parameters.Add(paramTickerId);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
            {
                return !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]) ? (int)ds.Tables[0].Rows[0][0] : Int32.MinValue;
            }
            else
            {
                return Int32.MinValue;
            }
        }

        private int CreateTimeseries(int tickerId, int unitId, int tsTypeId, int timeframeId, List<string> columns, SqlConnection conn)
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

            for (int i = 0; i < columns.Count; ++i)
            {
                var paramColumn = new SqlParameter(string.Format("IN_Column_{0}_Name", i + 1), SqlDbType.VarChar, 255, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, columns[i]);
                cmd.Parameters.Add(paramColumn);
            }

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
            {
                return !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]) ? (int)ds.Tables[0].Rows[0][0] : Int32.MinValue;
            }
            else
            {
                return Int32.MinValue;
            }

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
            cmd.Parameters.Add(paramTickerName);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            if (ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
            {
                return !DBNull.Value.Equals(ds.Tables[0].Rows[0][0]) ? (string)ds.Tables[0].Rows[0][0] : null;
            }
            else
            {
                return null;
            }
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
