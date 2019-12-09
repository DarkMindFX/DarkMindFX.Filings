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
    }
}
