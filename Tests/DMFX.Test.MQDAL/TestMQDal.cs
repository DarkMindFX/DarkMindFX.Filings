using DMFX.MQDAL;
using DMFX.MQInterfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.MQDAL
{
    [TestFixture]
    public class TestMQDal : TestBase
    {
        SqlConnection _conn = null;

        [SetUp]
        public void SetUp()
        {
            _conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionStringMsgBus"]);
            _conn.Open();
        }

        [TearDown]
        public void TearDown()
        {
            _conn.Close();
        }

        [Test]
        public void RegisterSubscriber_Success()
        {
            RunInitSql("000.RegisterSubscriber_Success", _conn);

            IMessageQueue mq = CreateMQ();

            IMQRegisterSubscriberParams paramsRegSubscr = mq.CreateRegisterSubscriberParams();
            paramsRegSubscr.SubscriberName = ConfigurationManager.AppSettings["SenderName"];
            paramsRegSubscr.SubscriberUrl = null;

            IMQRegisterSubscriberResult result = mq.RegisterSubscriber(paramsRegSubscr);

            RunFinalizeSql("000.RegisterSubscriber_Success", _conn);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.Greater(result.SubscriberId, 0);
        }

        [Test]
        public void RegisterSubscriber_AlreadyExists()
        {
            RunInitSql("001.RegisterSubscriber_AlreadyExists", _conn);

            IMessageQueue mq = CreateMQ();

            IMQRegisterSubscriberParams paramsRegSubscr = mq.CreateRegisterSubscriberParams();
            paramsRegSubscr.SubscriberName = ConfigurationManager.AppSettings["SenderName"];
            paramsRegSubscr.SubscriberUrl = null;

            IMQRegisterSubscriberResult result = mq.RegisterSubscriber(paramsRegSubscr);

            RunFinalizeSql("001.RegisterSubscriber_AlreadyExists", _conn);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Errors);
            Assert.AreEqual(result.SubscriberId, 0);
        }

        [Test]
        public void CreateChannel_Success()
        {
            RunInitSql("010.CreateChannel_Success", _conn);

            IMessageQueue mq = CreateMQ();

            IMQCreateChannelParams paramsCreateChannel = mq.CreateCreateChannelParams();
            paramsCreateChannel.ChannelName = ConfigurationManager.AppSettings["ChannelName"];
            paramsCreateChannel.MessageTimeout = TimeSpan.FromMinutes(30);

            IMQCreateChannelResult result = mq.CreateChannel(paramsCreateChannel);

            RunFinalizeSql("010.CreateChannel_Success", _conn);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.Greater(result.ChannelId, 0);
        }

        [Test]
        public void CreateChannel_AlreadyExists()
        {
            RunInitSql("011.CreateChannel_AlreadyExists", _conn);

            IMessageQueue mq = CreateMQ();

            IMQCreateChannelParams paramsCreateChannel = mq.CreateCreateChannelParams();
            paramsCreateChannel.ChannelName = ConfigurationManager.AppSettings["ChannelName"];
            paramsCreateChannel.MessageTimeout = TimeSpan.FromMinutes(30);

            IMQCreateChannelResult result = mq.CreateChannel(paramsCreateChannel);

            RunFinalizeSql("011.CreateChannel_AlreadyExists", _conn);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.Greater(result.ChannelId, 0);
        }

        [Test]
        public void SubscribeToChannel_Success()
        {
            RunInitSql("020.SubscribeToChannel_Success", _conn);

            IMessageQueue mq = CreateMQ();

            // getting channel ID
            IMQGetChannelIdParams paramsGetChannelId = mq.CreateGetChannelIdParams();
            paramsGetChannelId.ChannelName = ConfigurationManager.AppSettings["ChannelName"];

            IMQGetChannelIdResult channelId = mq.GetChannelId(paramsGetChannelId);

            // getting subscriber ID
            IMQGetSubscriberIdParams paramsGetSubscriberId = mq.CreateGetSubscriberIdParams();
            paramsGetSubscriberId.SubscriberName = ConfigurationManager.AppSettings["SenderName"];

            IMQGetSubscriberIdResult subscriberId = mq.GetSubscriberId(paramsGetSubscriberId);

            // subscribing
            IMQSubscribeParams paramSubscribe = mq.CreateSubscribeParams();
            paramSubscribe.ChannelId = (long)channelId.ChannelId;
            paramSubscribe.SubscriberId = (long)subscriberId.SubscriberId;

            var result = mq.Subscribe(paramSubscribe);

            RunFinalizeSql("020.SubscribeToChannel_Success", _conn);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
       
        }

        #region Support methods
        private IMessageQueue CreateMQ()
        {
            var mq = new DbMessageQueue();
            IMQInitParams paramsInit = mq.CreateInitParams();
            paramsInit.Params["ConnectionString"] = ConfigurationManager.AppSettings["ConnectionStringMsgBus"];
            mq.Init(paramsInit);

            return mq;
        }
        #endregion


    }
}
