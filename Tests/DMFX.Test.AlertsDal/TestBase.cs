using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.AlertsDal
{
    public class TestBase
    {     

        public bool RunInitSql(string scenario, SqlConnection connection)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Init.sql");
            string sql = ReadFileContent(path).Trim();

            if (sql != null && connection != null)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteSql(sql, connection);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RunFinalizeSql(string scenario, SqlConnection connection)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Finalize.sql");
            string sql = ReadFileContent(path);

            if (sql != null && connection != null)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteSql(sql, connection);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private string ReadFileContent(string path)
        {
            string content = null;
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
            }

            return content;
        }

        public bool ExecuteSql(string sql, SqlConnection conn)
        {
            if (!string.IsNullOrEmpty(sql.Trim()))
            {

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = conn;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}
