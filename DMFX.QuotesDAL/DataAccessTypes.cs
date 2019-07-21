using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.QuotesDAL
{
    class DataAccessTypes
    {
        static public DataTable CreateLoadTimeSeriesMetadataTable()
        {
            DataTable dtMetadata = new DataTable();
            dtMetadata.Columns.Add(new DataColumn("Even_Date", typeof(DateTime)));
            dtMetadata.Columns.Add(new DataColumn("Col_1", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_2", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_3", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_4", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_5", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_6", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_7", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_8", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_9", typeof(decimal)));
            dtMetadata.Columns.Add(new DataColumn("Col_10", typeof(decimal)));

            return dtMetadata;
        }

        
    }
}
