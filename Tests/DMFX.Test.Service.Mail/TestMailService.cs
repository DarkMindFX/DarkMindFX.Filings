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

namespace DMFX.Test.Service.Mail
{
    [TestFixture]
    public class TestMailService : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            string accountKey = ConfigurationManager.AppSettings["AccountKey"];
            string serviceUrl = ConfigurationManager.AppSettings["MailServiceURL"];

            // Filings DB
            SqlConnection sqlConnFilings = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringFilings"]);
            sqlConnFilings.Open();

            // Accounts DB
            SqlConnection sqlConnAccounts = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringAccounts"]);
            sqlConnAccounts.Open();

            Dictionary<string, SqlConnection> connections = new Dictionary<string, SqlConnection>();
            connections.Add("ConnectionStringFilings", sqlConnFilings);
            connections.Add("ConnectionStringAccounts", sqlConnAccounts);

            Init(serviceUrl, connections, accountKey);

            string sqlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "TestInit.sql");
            string initSql = File.ReadAllText(sqlPath);
            ExecuteSql(initSql, Connections["ConnectionStringAccounts"]);

            this.SessionToken = ConfigurationManager.AppSettings["MailServiceSessionToken"];

        }

        [TestCase("000.SendMail.AccountCreatedConfirmation.Success")]
        public void SendMail_Success(string name)
        {

            SendMail request = PrepareRequest<SendMail>(name);

            SendMailResponse response = Post<SendMail, SendMailResponse>("SendMail", request);

            Assert.IsTrue(response.Success, "Failed to send email");

        }
    }
}
