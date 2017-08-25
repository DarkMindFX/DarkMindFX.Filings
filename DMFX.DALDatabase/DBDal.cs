using DMFX.Interfaces;
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
            SqlParameter paramMetadata = new SqlParameter("@LMD", SqlDbType.Structured);
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

        private DataTable ConvertToFilingDataTable(List<InsertFilingDetailsParams.FilingRecord> records)
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

                dtFilingData.Rows.Add(rowFilingData);
            }

            return dtFilingData;
        }
    }
}
