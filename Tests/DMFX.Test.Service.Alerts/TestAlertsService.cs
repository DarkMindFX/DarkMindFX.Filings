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

namespace DMFX.Test.Service.Alerts
{
    [TestFixture]
    public class TestAlertsService : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            string accountKey = ConfigurationManager.AppSettings["AccountKey"];
            string serviceUrl = ConfigurationManager.AppSettings["AlertsServiceURL"];

            // Alerts DB
            SqlConnection sqlConnAlerts = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringAlerts"]);
            sqlConnAlerts.Open();

            Dictionary<string, SqlConnection> connections = new Dictionary<string, SqlConnection>();
            connections.Add("ConnectionStringAlerts", sqlConnAlerts);

            Init(serviceUrl, connections, accountKey);

            string sqlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "TestInit.sql");
            string initSql = File.ReadAllText(sqlPath);
            ExecuteSql(initSql, Connections["ConnectionStringAlerts"]);

        }

        [TestCase("000.GetAlertsTypes.Success")]
        public void GetAlertsTypes_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            GetAlertsTypes request = PrepareRequest<GetAlertsTypes>(name);

            GetAlertsTypesResponse response = Post<GetAlertsTypes, GetAlertsTypesResponse>("GetAlertsTypes", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, true, "GetAlertsTypes call failed");
            Assert.IsNotEmpty(response.Payload.Types, "Types is empty");
        }

        [TestCase("010.GetNotificationTypes.Success")]
        public void GetNotificationTypes_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            GetNotificationTypes request = PrepareRequest<GetNotificationTypes>(name);

            GetNotificationTypesResponse response = Post<GetNotificationTypes, GetNotificationTypesResponse>("GetNotificationTypes", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, true, "GetNotificationTypes call failed");
            Assert.IsNotEmpty(response.Payload.Types, "Types is empty");
        }

        [TestCase("020.GetAccountAlerts.Success")]
        public void GetAccountAlerts_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            GetAccountAlerts request = PrepareRequest<GetAccountAlerts>(name);

            GetAccountAlertsResponse response = Post<GetAccountAlerts, GetAccountAlertsResponse>("GetAccountAlerts", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, true, "GetAccountAlerts call failed");
            Assert.IsNotEmpty(response.Payload.Alerts, "Alerts is empty");
            Assert.AreEqual(response.Payload.Alerts.Count, 2, "Invalid alerts count");
        }

        [TestCase("021.GetAccountAlerts.InvalidKey")]
        public void GetAccountAlerts_InvalidKey(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            GetAccountAlerts request = PrepareRequest<GetAccountAlerts>(name);

            GetAccountAlertsResponse response = Post<GetAccountAlerts, GetAccountAlertsResponse>("GetAccountAlerts", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, true, "GetAccountAlerts call failed unexpectedly");
            Assert.IsEmpty(response.Payload.Alerts, "Alerts are not empty");
        }

        [TestCase("022.GetAccountAlerts.InvalidSessionToken")]
        public void GetAccountAlerts_Fail(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            GetAccountAlerts request = PrepareRequest<GetAccountAlerts>(name);

            GetAccountAlertsResponse response = Post<GetAccountAlerts, GetAccountAlertsResponse>("GetAccountAlerts", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, false, "GetAccountAlerts call succeeded unexpectedly");
            Assert.IsEmpty(response.Payload.Alerts, "Alerts are not empty");
        }

        [TestCase("030.AddAccountAlerts.Success")]
        public void AddAccountAlerts_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            AddAccountAlerts request = PrepareRequest<AddAccountAlerts>(name);

            AddAccountAlertsResponse response = Post<AddAccountAlerts, AddAccountAlertsResponse>("AddAccountAlerts", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, true, "AddAccountAlerts call failed");
            Assert.IsNull(response.Errors.FirstOrDefault(e => e.Type == Interfaces.EErrorType.Error), "Errors are not empty");
 
        }

        [TestCase("031.AddAccountAlerts.InvalidSessionToken")]
        public void AddAccountAlerts_InvalidSessionToken(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            AddAccountAlerts request = PrepareRequest<AddAccountAlerts>(name);

            AddAccountAlertsResponse response = Post<AddAccountAlerts, AddAccountAlertsResponse>("AddAccountAlerts", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, false, "AddAccountAlerts call succeeded unexpectedly");
            Assert.IsNotNull(response.Errors.FirstOrDefault(e => e.Type == Interfaces.EErrorType.Error), "Errors are empty");
            Assert.AreEqual(response.Errors[0].Code, Interfaces.EErrorCodes.InvalidSession, "Wrong error code returned");

        }

        [TestCase("040.UpdateAccountAlerts.Success")]
        public void UpdateAccountAlerts_Success(string name)
        {
            RunInitSql(name, "ConnectionStringAlerts");

            // getting alerts to obtain IDs
            GetAccountAlerts getAlertsReq = new GetAccountAlerts()
            {
                AccountKey = ConfigurationManager.AppSettings["AccountKey"],
                SessionToken = ConfigurationManager.AppSettings["SessionToken"],
                RequestID = Guid.NewGuid().ToString()
            };

            GetAccountAlertsResponse getAlertsResp = Post<GetAccountAlerts, GetAccountAlertsResponse>("GetAccountAlerts", getAlertsReq);

            // setting proper IDs
            UpdateAccountAlerts request = PrepareRequest<UpdateAccountAlerts>(name);

            foreach(var a in request.Alerts)
            {
                var alert = getAlertsResp.Payload.Alerts.FirstOrDefault(x => a.Name.Contains(x.Name));
                if(alert != null)
                {
                    a.ID = alert.ID;
                }
            }

            UpdateAccountAlertsResponse response = Post<UpdateAccountAlerts, UpdateAccountAlertsResponse>("UpdateAccountAlerts", request);
            RunFinalizeSql(name, "ConnectionStringAlerts");

            Assert.AreEqual(response.Success, true, "UpdateAccountAlerts failed succeeded unexpectedly");
            Assert.IsNull(response.Errors.FirstOrDefault(e => e.Type == Interfaces.EErrorType.Error), "Errors are not empty");
 
        }
    }
}
