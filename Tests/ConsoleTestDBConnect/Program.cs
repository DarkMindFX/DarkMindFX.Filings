using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestDBConnect
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SqlConnection conn = new SqlConnection("Server=198.71.227.2;Database=globus000_DMFXFilings;User Id=dmfx_dev02;Password = DMFXFilings2017;");
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Connection opened successfully");
                    conn.Close();
                }
                else
                {
                    Console.WriteLine("Connection open failed");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }
}
