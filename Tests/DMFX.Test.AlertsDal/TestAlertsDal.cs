using DMFX.AlertsInterfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.AlertsDal
{
    [TestFixture]
    public class TestAlertsDal : TestBase
    {
        private SqlConnection _conn = null;
        [SetUp]
        public void Setup()
        {
            _conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringAlerts"]);
            _conn.Open();

        }

        [TearDown]
        public void TearDown()
        {
            if(_conn != null)
            {
                _conn.Close();
            }

        }

        [Test]
        public void GetAlertTypes_Success()
        {
            IAlertsDal dal = PrepareAlertsDal();
            IAlertsDalGetAlertTypesParams getTypesParams = dal.CreateGetAlertTypesParams();

            var getTypesResult = dal.GetAlertTypes(getTypesParams);

            Assert.IsTrue(getTypesResult.Success);
            Assert.IsNotNull(getTypesResult.Types != null);
            Assert.IsNotEmpty(getTypesResult.Types);
        }

        [Test]
        public void GetAlertNotificationTypes_Success()
        {
            IAlertsDal dal = PrepareAlertsDal();
            IAlertsDalGetAlertNotificationTypesParams getTypesParams = dal.CreateGetAlertNotificationTypesParams();

            var getTypesResult = dal.GetAlertNotificationTypes(getTypesParams);

            Assert.IsTrue(getTypesResult.Success);
            Assert.IsNotNull(getTypesResult.Types != null);
            Assert.IsNotEmpty(getTypesResult.Types);
        }

        [Test]
        public void GetAccountSubscriptions_Success()
        {

            RunInitSql("000.GetAccountSubscriptions_Success", _conn);

            IAlertsDal dal = PrepareAlertsDal();
            IAlertsDalGetAccountSubscriptionsParams getGetSubsParams = dal.CreateGetAccountSubscrParams();
            getGetSubsParams.AccountKey = ConfigurationManager.AppSettings["AccountKey"];

            var getSubsResult = dal.GetAccountSubscriptions(getGetSubsParams);

            RunFinalizeSql("000.GetAccountSubscriptions_Success", _conn);

            Assert.IsTrue(getSubsResult.Success);
            Assert.IsNotNull(getSubsResult.Subscriptions != null);
            Assert.IsNotEmpty(getSubsResult.Subscriptions);

        }

        [Test]
        public void GetAccountSubscriptions_InvalidKey()
        {
            IAlertsDal dal = PrepareAlertsDal();
            IAlertsDalGetAccountSubscriptionsParams getGetSubsParams = dal.CreateGetAccountSubscrParams();
            getGetSubsParams.AccountKey = ConfigurationManager.AppSettings["InvalidAccountKey"];

            var getSubsResult = dal.GetAccountSubscriptions(getGetSubsParams);

            Assert.IsTrue(getSubsResult.Success);
            Assert.IsNotNull(getSubsResult.Subscriptions != null);
            Assert.IsEmpty(getSubsResult.Subscriptions);

        }

        #region Support methods
        IAlertsDal PrepareAlertsDal()
        {
            IAlertsDal dal = new AlertsDAL.AlertsDALMSSQL();
            IAlertsDalInitParams initParams = dal.CreateInitParams();
            initParams.Parameters["ConnectionStringAlerts"] = ConfigurationManager.AppSettings["ConnectionStringAlerts"];

            dal.Init(initParams);

            return dal;
        }
        #endregion
    }
}
