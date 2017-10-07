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
    public class TestAccountsService : TestBase
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

        [TestCase("000.CreateUserAccount.Success")]
        public void CreateUserAccount_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            CreateAccount request = PrepareRequest<CreateAccount>(name);

            CreateAccountResponse response = Post<CreateAccount, CreateAccountResponse>("CreateAccount", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, true, "CreateAccount call failed");
            Assert.IsNotEmpty(response.AccountKey, "AccountKey is empty");
            

        }

        [TestCase("001.CreateUserAccount.AlreadyExists")]
        public void CreateUserAccount_AlreadyExists(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            CreateAccount request = PrepareRequest<CreateAccount>(name);

            CreateAccountResponse response = Post<CreateAccount, CreateAccountResponse>("CreateAccount", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, false, "User with duplicate name was successfully created");
            Assert.IsNotEmpty(response.Errors, "Error was not returned");
            Assert.AreEqual(response.Errors[0].Code, Interfaces.EErrorCodes.UserAccountExists, "Incorrect error code returned");
            Assert.IsTrue( string.IsNullOrEmpty(response.AccountKey), "AccountKey is not empty");


        }
    }
}
