using DMFX.Interfaces;
using DMFX.Service.DTO;
using DMFX.Test.Service.Common;
using NUnit.Framework;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Service.Filings
{
    [TestFixture]
    public class TestFilingsService : TestBase
    {

        [SetUp]  
        public void SetUp()
        {
            string accountKey = ConfigurationManager.AppSettings["AccountKey"];
            string serviceUrl = ConfigurationManager.AppSettings["FilingsServiceURL"];

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

            // Initializing session
            InitSession(ConfigurationManager.AppSettings["AccountsServiceURL"]);
        }

        [TearDown]
        public void TearDown()
        {
            // Closing session
            CloseSession(ConfigurationManager.AppSettings["AccountsServiceURL"]);

            string sqlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "TestTeardown.sql");
            string teardownSql = File.ReadAllText(sqlPath);
            ExecuteSql(teardownSql, Connections["ConnectionStringAccounts"]);

            // closing connections
            foreach (var c in Connections.Values)
            {
                if (c != null && c.State != System.Data.ConnectionState.Closed)
                {
                    c.Close();
                }
            }

            Connections.Clear();
        }

        [TestCase("000.GetRegulators.Success")]
        public void GetRegulators_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetRegulators request = PrepareRequest<GetRegulators>(name);

            GetRegulatorsResponse response = Post<GetRegulators, GetRegulatorsResponse>("GetRegulators", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetRegulators call failed");
            Assert.Greater(response.Regulators.Count, 0, "Empty regulator list returned");
            Assert.AreNotEqual(response.Regulators.FirstOrDefault(i => i.Code == "SEC"), null, "SEC is not present on the list");
        }

        [TestCase("001.GetRegulators.InvalidSession")]
        public void GetRegulators_InvalidSession(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetRegulators request = PrepareRequest<GetRegulators>(name);

            request.SessionToken = Guid.NewGuid().ToString();

            GetRegulatorsResponse response = Post<GetRegulators, GetRegulatorsResponse>("GetRegulators", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, false, "GetRegulators succeeded with invalid session");
            Assert.AreEqual(response.Regulators.Count, 0, "Regulator list is not empty");
            Assert.IsNotEmpty(response.Errors, "Errors are empty in the response");
            Assert.AreEqual(response.Errors[0].Code, EErrorCodes.InvalidSession, "Incorrect error code returned");
            
        }

        [TestCase("010.GetCompanies.Success")]
        public void GetCompanies_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanies request = PrepareRequest<GetCompanies>(name);

            GetCompaniesResponse response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanies call failed");
            Assert.Greater(response.Companies.Count, 0, "Empty companies list returned");
        }

        [TestCase("011.GetCompanies.InvalidSession")]
        public void GetCompanies_InvalidSession(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanies request = PrepareRequest<GetCompanies>(name);

            request.SessionToken = Guid.NewGuid().ToString();

            GetCompaniesResponse response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, false, "GetCompanies succeeded with invalid session");
            Assert.AreEqual(response.Companies.Count, 0, "Regulator list is not empty");
            Assert.IsNotEmpty(response.Errors, "Errors are empty in the response");
            Assert.AreEqual(response.Errors[0].Code, EErrorCodes.InvalidSession, "Incorrect error code returned");

        }

        [TestCase("012.GetCompanies.InvalidRegulatorCode")]
        public void GetCompanies_InvalidRegulatorCode(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanies request = PrepareRequest<GetCompanies>(name);

            GetCompaniesResponse response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanies call failed");
            Assert.AreEqual(response.Companies.Count, 0, "Non-empty list of companies returned for invalid regulator code");
        }

        [TestCase("020.GetCompanyFilingsInfo.Success")]
        public void GetCompanyFilingsInfo_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanyFilingsInfo call failed");
            Assert.Greater(response.Filings.Count, 0, "Empty filings list returned");
        }

        [TestCase("021.GetCompanyFilingsInfo.InvalidSession")]
        public void GetCompanyFilingsInfo_InvalidSession(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            request.SessionToken = Guid.NewGuid().ToString();

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, false, "GetCompanyFilingsInfo succeeded with invalid session");
            Assert.AreEqual(response.Filings.Count, 0, "Filings list is not empty");
            Assert.IsNotEmpty(response.Errors, "Errors are empty in the response");
            Assert.AreEqual(response.Errors[0].Code, EErrorCodes.InvalidSession, "Incorrect error code returned");

        }

        [TestCase("022.GetCompanyFilingsInfo.InvalidCompanyCode")]
        public void GetCompanyFilingsInfo_InvalidCompanyCode(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanyFilingsInfo call failed");
            Assert.AreEqual(response.Filings.Count, 0, "Non-empty list of companies returned for invalid regulator code");
        }

        [TestCase("023.GetCompanyFilingsInfo.InvalidRegulatorCode")]
        public void GetCompanyFilingsInfo_InvalidRegulatorCode(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanyFilingsInfo call failed");
            Assert.AreEqual(response.Filings.Count, 0, "Non-empty list of companies returned for invalid regulator code");
        }

        [TestCase("024.GetCompanyFilingsInfo.10-Q.Success")]
        public void GetCompanyFilingsInfo_10Q_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanyFilingsInfo call failed");
            Assert.Greater(response.Filings.Count, 0, "Empty filings list returned");
            Assert.IsNotEmpty(response.Filings.Where(r => r.Type == "10-Q"), "Reports of type 10-Q were not returned");
        }

        [TestCase("025.GetCompanyFilingsInfo.10-K.Success")]
        public void GetCompanyFilingsInfo_10K_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanyFilingsInfo call failed");
            Assert.Greater(response.Filings.Count, 0, "Empty filings list returned");
            Assert.IsNotEmpty(response.Filings.Where(r => r.Type == "10-K"), "Reports of type 10-K were not returned");
        }

        [TestCase("026.GetCompanyFilingsInfo.MultipleTypes.Success")]
        public void GetCompanyFilingsInfo_MultipleTypes_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetCompanyFilingsInfo request = PrepareRequest<GetCompanyFilingsInfo>(name);

            GetCompanyFilingsInfoResponse response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetCompanyFilingsInfo call failed");
            Assert.Greater(response.Filings.Count, 0, "Empty filings list returned");
            Assert.IsNotEmpty(response.Filings.Where(r => r.Type == "10-K"), "Reports of type 10-K were not returned");
            Assert.IsNotEmpty(response.Filings.Where(r => r.Type == "10-Q"), "Reports of type 10-Q were not returned");
        }

        [TestCase("030.GetFilingData.Success")]
        public void GetFilingData_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetFilingData request = PrepareRequest<GetFilingData>(name);

            GetFilingDataResponse response = Post<GetFilingData, GetFilingDataResponse>("GetFilingData", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetFilingData call failed");
            Assert.AreNotEqual(response.FilingData.Count, 0, "Empty listof values returned");
        }

        [TestCase("030.GetFilingData.Values.Success")]
        public void GetFilingData_Values_Success(string name)
        {
            RunInitSql(name, "ConnectionStringFilings");

            GetFilingData request = PrepareRequest<GetFilingData>(name);

            GetFilingDataResponse response = Post<GetFilingData, GetFilingDataResponse>("GetFilingData", request);

            RunFinalizeSql(name, "ConnectionStringFilings");

            Assert.AreEqual(response.Success, true, "GetFilingData call failed");
            Assert.AreNotEqual(response.FilingData.Count, 0, "Empty listof values returned");
        }


    }
}
