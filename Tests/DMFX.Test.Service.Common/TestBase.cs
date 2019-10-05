using System;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using ServiceStack;
using DMFX.Service.DTO;
using System.Collections.Generic;
using ServiceStack.ServiceClient.Web;

namespace DMFX.Test.Service.Common
{
    public class TestBase
    {
        protected Dictionary<string, SqlConnection> Connections
        {
            get;
            set;
        }
    
        protected JsonServiceClient Client
        {
            get;
            set;
        }

        protected string AccountKey
        {
            get;
            set;
        }

        protected string SessionToken
        {
            get;
            set;
        }

        public void Init(string host, Dictionary<string, SqlConnection> connections, string accountKey)
        {
            // initializing session
            Client = new JsonServiceClient(host);
            Connections = connections;
            AccountKey = accountKey;

        }

        protected void InitSession(string url)
        {
            // Initializing session
            using (var accountClient = new JsonServiceClient(url))
            {
                InitSession reqInit = new InitSession();
                reqInit.RequestID = System.Guid.NewGuid().ToString();
                reqInit.AccountKey = AccountKey;

                InitSessionResponse resInit = accountClient.Post<InitSessionResponse>("InitSession", reqInit);

                if (resInit.Success)
                {
                    SessionToken = resInit.SessionToken;
                }
                else
                {
                    throw new Exception(string.Format("Init call error:{0}", resInit.Errors[0].Code));
                }
            }
        }

        protected void CloseSession(string url)
        {
            // Initializing session
            using (var accountClient = new JsonServiceClient(url))
            {
                CloseSession reqInit = new CloseSession();
                reqInit.RequestID = System.Guid.NewGuid().ToString();
                reqInit.SessionToken = SessionToken;

                CloseSessionResponse resClose = accountClient.Post<CloseSessionResponse>("CloseSession", reqInit);

                if (resClose.Success)
                {
                    SessionToken = null;
                }
                else
                {
                    throw new Exception(string.Format("CloseSession call error:{0}", resClose.Errors[0].Code));
                }
            }
        }

        public bool RunInitSql(string scenario, string connName)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Init.sql");
            string sql = ReadFileContent(path).Trim();

            if (sql != null && Connections != null && Connections[connName].State == System.Data.ConnectionState.Open)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteSql(sql, Connections[connName]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RunFinalizeSql(string scenario, string connName)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", scenario, "Finalize.sql");
            string sql = ReadFileContent(path);

            if (sql != null && Connections != null && Connections[connName].State == System.Data.ConnectionState.Open)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    ExecuteSql(sql, Connections[connName]);
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
            if (request is RequestBase && string.IsNullOrEmpty((request as RequestBase).SessionToken))
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
