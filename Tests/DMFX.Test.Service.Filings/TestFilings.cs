using DMFX.Interfaces;
using DMFX.Service.DTO;
using NUnit.Framework;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Service.Filings
{
    [TestFixture]
    public class TestFilings : TestBase
    {

        [SetUp]  
        public void SetUp()
        {
            string accountKey = ConfigurationManager.AppSettings["AccountKey"];
            string serviceUrl = ConfigurationManager.AppSettings["ServiceURL"];
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            sqlConn.Open();

            Init(serviceUrl, sqlConn, accountKey);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [TestCase("000.GetRegulators.Success")]
        public void GetRegulators_Success(string name)
        {
            RunInitSql(name);

            GetRegulators request = PrepareRequest<GetRegulators>(name);

            GetRegulatorsResponse response = Post<GetRegulators, GetRegulatorsResponse>("GetRegulators", request);

            RunFinalizeSql(name);

            Assert.AreEqual(response.Success, true, "GetRegulators call failed");
            Assert.Greater(response.Regulators.Count, 0, "Empty regulator list returned");
            Assert.AreNotEqual(response.Regulators.FirstOrDefault(i => i.Code == "SEC"), null, "SEC is not present on the list");
        }

        [TestCase("001.GetRegulators.InvalidSession")]
        public void GetRegulators_InvalidSession(string name)
        {
            RunInitSql(name);

            GetRegulators request = PrepareRequest<GetRegulators>(name);

            request.SessionToken = Guid.NewGuid().ToString();

            GetRegulatorsResponse response = Post<GetRegulators, GetRegulatorsResponse>("GetRegulators", request);

            RunFinalizeSql(name);

            Assert.AreEqual(response.Success, false, "GetRegulators succeeded with invalid session");
            Assert.AreEqual(response.Regulators.Count, 0, "Regulator list is not empty");
            Assert.IsNotEmpty(response.Errors, "Errors are empty in the response");
            Assert.AreEqual(response.Errors[0].Code, EErrorCodes.InvalidSession, "Incorrect error code returned");
            
        }

        [TestCase("010.GetCompanies.Success")]
        public void GetCompanies_Success(string name)
        {
            RunInitSql(name);

            GetCompanies request = PrepareRequest<GetCompanies>(name);

            GetCompaniesResponse response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            RunFinalizeSql(name);

            Assert.AreEqual(response.Success, true, "GetCompanies call failed");
            Assert.Greater(response.Companies.Count, 0, "Empty companies list returned");
        }

        [TestCase("011.GetCompanies.InvalidSession")]
        public void GetCompanies_InvalidSession(string name)
        {
            RunInitSql(name);

            GetCompanies request = PrepareRequest<GetCompanies>(name);

            request.SessionToken = Guid.NewGuid().ToString();

            GetCompaniesResponse response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            RunFinalizeSql(name);

            Assert.AreEqual(response.Success, false, "GetCompanies succeeded with invalid session");
            Assert.AreEqual(response.Companies.Count, 0, "Regulator list is not empty");
            Assert.IsNotEmpty(response.Errors, "Errors are empty in the response");
            Assert.AreEqual(response.Errors[0].Code, EErrorCodes.InvalidSession, "Incorrect error code returned");

        }

        [TestCase("012.GetCompanies.InvalidRegulatorCode")]
        public void GetCompanies_InvalidRegulatorCode(string name)
        {
            RunInitSql(name);

            GetCompanies request = PrepareRequest<GetCompanies>(name);

            GetCompaniesResponse response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            RunFinalizeSql(name);

            Assert.AreEqual(response.Success, true, "GetCompanies call failed");
            Assert.AreEqual(response.Companies.Count, 0, "Non-empty list of companies returned for invalid regulator code");
        }


    }
}
