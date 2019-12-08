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

        [Test]
        public void AddAlertSubscription_Success()
        {
            RunInitSql("010.AddAccountSubscriptions_Success", _conn);

            IAlertsDal dal = PrepareAlertsDal();
            IAlertsDalAddAccountSubscrParams addSubParam = dal.CreateAddAccountSubscrParams();
            addSubParam.SubscriptonDetails = new Subscription();
            addSubParam.SubscriptonDetails.AccountKey = ConfigurationManager.AppSettings["AccountKey"];
            addSubParam.SubscriptonDetails.Name = ConfigurationManager.AppSettings["TestSubscriptionName"];
            addSubParam.SubscriptonDetails.NotificationTypeId = Int32.Parse(ConfigurationManager.AppSettings["NotificatioType_Id_CustomUrl"]);
            addSubParam.SubscriptonDetails.TypeId = Int32.Parse(ConfigurationManager.AppSettings["Alert_Id_10K"]);
            addSubParam.SubscriptonDetails.SubscribedDttm = DateTime.UtcNow;

            addSubParam.SubscriptonDetails.SubscriptionData.Add("Url", "http://customurl.com/");

            IAlertsDalAddAccountSubscrResult addSubsResult = dal.AddAlertSubscription(addSubParam);

            RunFinalizeSql("010.AddAccountSubscriptions_Success", _conn);

            Assert.IsTrue(addSubsResult.Success);

        }

        [Test]
        public void UpdateAlertSubscription_Success()
        {
            RunInitSql("020.UpdateAccountSubscriptions_Success", _conn);

            IAlertsDal dal = PrepareAlertsDal();

            IAlertsDalGetAccountSubscriptionsParams getGetSubsParams = dal.CreateGetAccountSubscrParams();
            getGetSubsParams.AccountKey = ConfigurationManager.AppSettings["AccountKey"];

            var getSubsResult = dal.GetAccountSubscriptions(getGetSubsParams);

            IAlertsDalUpdateAccountSubscrParams updSubParam = dal.CreateUpdateAccountSubscrParams();
            updSubParam.SubscriptonDetails = new Subscription();
            updSubParam.SubscriptonDetails.Id = getSubsResult.Subscriptions[0].Id;
            updSubParam.SubscriptonDetails.AccountKey = ConfigurationManager.AppSettings["AccountKey"];
            updSubParam.SubscriptonDetails.Name = "Test Subscription 02 Updated";
            updSubParam.SubscriptonDetails.NotificationTypeId = Int32.Parse(ConfigurationManager.AppSettings["NotificatioType_Id_CustomUrl"]);
            updSubParam.SubscriptonDetails.SubscribedDttm = DateTime.UtcNow;

            updSubParam.SubscriptonDetails.SubscriptionData.Add("Url", "http://customurl.com/");

            IAlertsDalUpdateAccountSubscrResult updSubsResult = dal.UpdateAlertSubscription(updSubParam);

            RunFinalizeSql("020.UpdateAccountSubscriptions_Success", _conn);

            Assert.IsTrue(updSubsResult.Success);

        }

        [Test]
        public void RemoveAlertSubscription_Success()
        {
            RunInitSql("030.RemoveAccountSubscription_Success", _conn);

            IAlertsDal dal = PrepareAlertsDal();

            IAlertsDalGetAccountSubscriptionsParams getGetSubsParams = dal.CreateGetAccountSubscrParams();
            getGetSubsParams.AccountKey = ConfigurationManager.AppSettings["AccountKey"];

            var getSubsResult = dal.GetAccountSubscriptions(getGetSubsParams);

            IAlertsDalRemoveAccountSubscrParams remSubParam = dal.CreateRemoveAccountSubscrParams();
            remSubParam.SubscriptionIds.Add(getSubsResult.Subscriptions[0].Id);

            IAlertsDalRemoveAccountSubscrResult updSubsResult = dal.RemoveAlertSubscription(remSubParam);

            RunFinalizeSql("030.RemoveAccountSubscription_Success", _conn);

            Assert.IsTrue(updSubsResult.Success);

        }

        [Test]
        public void SetAlertSubscriptionStatus_Success()
        {
            RunInitSql("040.SetAccountSubscriptionStatus_Success", _conn);

            IAlertsDal dal = PrepareAlertsDal();

            IAlertsDalGetAccountSubscriptionsParams getGetSubsParams = dal.CreateGetAccountSubscrParams();
            getGetSubsParams.AccountKey = ConfigurationManager.AppSettings["AccountKey"];

            var getSubsResult = dal.GetAccountSubscriptions(getGetSubsParams);

            IAlertsDalSetAlertStatusParams updSubParam = dal.CreateSetAlertStatusParams();
            getSubsResult.Subscriptions[0].StatusId = Int32.Parse(ConfigurationManager.AppSettings["Status_Id_Disabled"]);
            updSubParam.Subscriptions.Add(getSubsResult.Subscriptions[0]);

            IAlertsDalSetAlertStatusResult updSubsResult = dal.SetAlertSubscriptionStatus(updSubParam);

            RunFinalizeSql("040.SetAccountSubscriptionStatus_Success", _conn);

            Assert.IsTrue(updSubsResult.Success);

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
