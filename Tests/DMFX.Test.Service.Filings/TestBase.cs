using System;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using ServiceStack;
using DMFX.Service.DTO;

namespace DMFX.Test.Service.Filings
{
    public class TestBase
    {
        private SqlConnection Connection
        {
            get;
            set;
        }

        public JsonServiceClient Client
        {
            get;
            set;
        }

        private string AccountKey
        {
            get;
            set;
        }

        private string SessionToken
        {
            get;
            set;
        }

        public void Init(string host, SqlConnection conn, string accountKey)
        {
            Client = new JsonServiceClient(host);
            Connection = conn;
            AccountKey = accountKey;

            InitSession reqInit = new InitSession();
            reqInit.RequestID = System.Guid.NewGuid().ToString();
            reqInit.AccountKey = AccountKey;

            InitSessionResponse resInit = Post<InitSession, InitSessionResponse>("InitSession", reqInit);

            if (resInit.Success)
            {
                SessionToken = resInit.SessionToken;
            }
            else
            {
                throw new Exception(string.Format("Init call error:{0}", resInit.Errors[0].Code));
            }

        }

        public bool RunInitSql(string scenario)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Init.sql");
            string sql = ReadFileContent(path).Trim();

            if (sql != null && Connection != null && Connection.State == System.Data.ConnectionState.Open)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteSql(sql);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RunFinalizeSql(string scenario)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Finalize.sql");
            string sql = ReadFileContent(path);

            if (sql != null && Connection != null && Connection.State == System.Data.ConnectionState.Open)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteSql(sql);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public TRequest PrepareRequest<TRequest>(string scenario)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Request.json");
            string requestJson = ReadFileContent(path);

            TRequest request = JsonConvert.DeserializeObject<TRequest>(requestJson);
            if (request is RequestBase)
            {
                (request as RequestBase).SessionToken = SessionToken;
            }
            return request;
        }

        public TResponse PrepareResponse<TResponse>(string scenario)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Response.json");
            string responseJson = ReadFileContent(path);

            TResponse response = JsonConvert.DeserializeObject<TResponse>(responseJson);

            return response;
        }

        // Basic methos to perform scenation testing
        public TResponse Post<TRequest, TResponse>(string method, TRequest request)
        {
            TResponse response = Client.Post<TResponse>(method, request);

            return response;
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

        private bool ExecuteSql(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = Connection;
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.ExecuteNonQuery();

            return true;
        }
    }
}
