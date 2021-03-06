﻿
using DMFX.Client.Accounts;
using DMFX.Service.DTO;
using DMFX.Test.Service.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Service.Accounts
{
    [TestFixture]
    class TestAccountsServiceClient : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            string accountKey = ConfigurationManager.AppSettings["AccountKey"];
            string serviceUrl = ConfigurationManager.AppSettings["AccountsServiceURL"];

            // Accounts DB
            SqlConnection sqlConnAccounts = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringAccounts"]);
            sqlConnAccounts.Open();

            Dictionary<string, SqlConnection> connections = new Dictionary<string, SqlConnection>();
            connections.Add("ConnectionStringAccounts", sqlConnAccounts);

            Init(serviceUrl, connections, accountKey);

            string sqlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "TestInit.sql");
            string initSql = File.ReadAllText(sqlPath);
            ExecuteSql(initSql, Connections["ConnectionStringAccounts"]);

        }

        [Test]
        public void TestClient_Echo_InvalidSessionToken()
        {
            ServiceClient client = new ServiceClient();

            Echo request = new Echo()
            {
                Message = "Account Client Echo",
                SessionToken = ConfigurationManager.AppSettings["InvalidSessionToken"],
                RequestID = Guid.NewGuid().ToString()
            };
            var response = client.PostEcho(request);

        }

        [Test]
        public void TestClient_InitSession_Success()
        {
            ServiceClient client = new ServiceClient();

            InitSession request = new InitSession()
            {
                AccountKey = ConfigurationManager.AppSettings["AccountKey"],
                RequestID = Guid.NewGuid().ToString()
            };
            var response = client.PostInitSession(request);

        }
    }
}
