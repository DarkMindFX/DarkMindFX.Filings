using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.DALDatabase
{
    class DataAccessTypes
    {
        static public DataTable CreateFilingMetadataTable()
        {
            DataTable dtMetadata = new DataTable();
            dtMetadata.Columns.Add(new DataColumn("Name", typeof(string)));
            dtMetadata.Columns.Add(new DataColumn("Value", typeof(string)));
            dtMetadata.Columns.Add(new DataColumn("DataType", typeof(string)));

            return dtMetadata;
        }

        static public DataTable CreateFilingDataTable()
        {
            DataTable dtFilingData = new DataTable();

            dtFilingData.Columns.Add(new DataColumn("Code", typeof(string)));
            dtFilingData.Columns.Add(new DataColumn("Value", typeof(Decimal)));
            dtFilingData.Columns.Add(new DataColumn("PeriodStart", typeof(DateTime)));
            dtFilingData.Columns.Add(new DataColumn("PeriodEnd", typeof(DateTime)));
            dtFilingData.Columns.Add(new DataColumn("UnitName", typeof(string)));
            dtFilingData.Columns.Add(new DataColumn("SourceFactId", typeof(string)));

            return dtFilingData;
        }
    }
}
