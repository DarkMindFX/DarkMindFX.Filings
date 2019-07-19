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

            string spName = "[SP_Get_TimeSeries]";
            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            // TODO: add list of SP params here
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            // TODO: uncomment when ready
            // da.Fill(ds);

            if (ds.Tables.Count >= 1)
            {
                BaseQuotesData qdata = new BaseQuotesData();
                qdata.Ticker = getQuotesParams.Tickers[0]; // TODO: for now only one ticker can be requested at a time
                qdata.Country = getQuotesParams.Country;
                qdata.TimeFrame = getQuotesParams.TimeFrame;
                // first table - company filings info records
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    BaseQuotesRecord qrec = new BaseQuotesRecord();
                    qrec.Time = (DateTime)row[0];
                    for (int i = 1; i < row.Table.Columns.Count - 1; ++i)
                    {
                        qrec[i] = (decimal)row[i];
                    }

                    qdata.AddRecord(qrec);
                }

                result.Quotes.Add(qdata);
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

            string spName = "[SP_Save_TimeSeries]";
            SqlConnection conn = OpenConnection("ConnectionStringTimeSeries");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            // TODO: add list of params here

            // TODO: uncomment when sp ready
            // cmd.ExecuteNonQuery();

            result.Success = true;

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
        private SqlConnection OpenConnection(string name)
        {
            SqlConnection conn = new SqlConnection(_dalParams.Parameters[name]);
            conn.Open();

            return conn;
        }

        #endregion

    }
}
