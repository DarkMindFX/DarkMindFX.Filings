using DMFX.Interfaces;
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

        [TestCase("010.Login.Success")]
        public void Login_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            Login request = PrepareRequest<Login>(name);

            LoginResponse response = Post<Login, LoginResponse>("Login", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, true, "Login failed");
            Assert.IsNotEmpty(response.SessionToken, "SessionToken is empty");
        }

        [TestCase("011.Login.InvalidUser")]
        public void Login_InvalidUser(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            Login request = PrepareRequest<Login>(name);

            LoginResponse response = Post<Login, LoginResponse>("Login", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, false, "Succeeded to login with invalid user name");
            Assert.IsNotEmpty(response.Errors, "Error was not returned");
            Assert.AreEqual(response.Errors[0].Code, Interfaces.EErrorCodes.UserAccountNotFound, "Incorrect error code returned");
            Assert.IsTrue(string.IsNullOrEmpty(response.SessionToken), "SessionToken is not empty");
        }

        [TestCase("012.Login.InvalidPassword")]
        public void Login_InvalidPassword(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            Login request = PrepareRequest<Login>(name);

            LoginResponse response = Post<Login, LoginResponse>("Login", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, false, "Succeeded to login with invalid password");
            Assert.IsNotEmpty(response.Errors, "Error was not returned");
            Assert.AreEqual(response.Errors[0].Code, Interfaces.EErrorCodes.UserAccountNotFound, "Incorrect error code returned");
            Assert.IsTrue(string.IsNullOrEmpty(response.SessionToken), "SessionToken is not empty");
        }

        [TestCase("020.InitSession.Success")]
        public void InitSession_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            InitSession request = PrepareRequest<InitSession>(name);

            InitSessionResponse response = Post<InitSession, InitSessionResponse>("InitSession", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, true, "Session was not initialized");
            Assert.IsEmpty(response.Errors, "Errors are not empty");

        }

        [TestCase("021.InitSession.InvalidAccountKey")]
        public void InitSession_InvalidAccountKey(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            InitSession request = PrepareRequest<InitSession>(name);

            InitSessionResponse response = Post<InitSession, InitSessionResponse>("InitSession", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, false, "Session was initialized with invalid key");
            Assert.IsNotEmpty(response.Errors, "Errors are empty");
            Assert.AreEqual(response.Errors[0].Code, EErrorCodes.UserAccountNotFound, "Incorrect error code returned");

        }

        [TestCase("030.CloseSession.Success")]
        public void CloseSession_Sussess(string name)
        {
            RunInitSql(name, "ConnectionStringAccounts");

            CloseSession request = PrepareRequest<CloseSession>(name);

            CloseSessionResponse response = Post<CloseSession, CloseSessionResponse>("CloseSession", request);

            RunFinalizeSql(name, "ConnectionStringAccounts");

            Assert.AreEqual(response.Success, true, "Session was not closed");
            Assert.IsEmpty(response.Errors, "Errors are not empty");
           

        }



    }
}
