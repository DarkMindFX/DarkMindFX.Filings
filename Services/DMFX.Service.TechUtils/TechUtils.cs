using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DMFX.Service.TechUtils
{
    public class TechUtils
    {

        private static string schema = "[dbo]";

        public void Sanitize()
        {
            string spName = "[SP_UTILS_Unlock_Db]";
            SqlConnection conn = OpenConnection("ConnectionStringFilings");


            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void ClearLayer(string layer)
        {
            string spName = "[SP_Clear_Db]";
            SqlConnection conn = OpenConnection("ConnectionStringFilings");


            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection = conn;

            // Company code
            SqlParameter paramLayer = new SqlParameter("@db", SqlDbType.NVarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current, layer);
            cmd.Parameters.Add(paramLayer);

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        private SqlConnection OpenConnection(string connName)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings[connName]);
            conn.Open();

            return conn;
        }
    }
}