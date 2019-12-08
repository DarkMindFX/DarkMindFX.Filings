using DMFX.AlertsInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.AlertsDAL
{
    class DataAccessTypes
    {
        static public DataTable CreateAlertsDataTable()
        {
            DataTable dtTable = new DataTable();
            dtTable.Columns.Add(new DataColumn("Subscription_Property_Name", typeof(string)));
            dtTable.Columns.Add(new DataColumn("Subscription_Property_Value", typeof(string)));

            return dtTable;
        }
    }
}
