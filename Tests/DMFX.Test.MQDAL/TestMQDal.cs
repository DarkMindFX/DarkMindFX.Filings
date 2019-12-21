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

        [Test]
        public void SubscribeToChannel_InvalidChannelId()
        {
            RunInitSql("021.SubscribeToChannel_InvalidChannelId", _conn);

            IMessageQueue mq = CreateMQ();

            // getting channel ID
            long invalidChannelId = 666000666;

            // getting subscriber ID
            IMQGetSubscriberIdParams paramsGetSubscriberId = mq.CreateGetSubscriberIdParams();
            paramsGetSubscriberId.SubscriberName = ConfigurationManager.AppSettings["SenderName"];

            IMQGetSubscriberIdResult subscriberId = mq.GetSubscriberId(paramsGetSubscriberId);

            // subscribing
            IMQSubscribeParams paramSubscribe = mq.CreateSubscribeParams();
            paramSubscribe.ChannelId = invalidChannelId;
            paramSubscribe.SubscriberId = (long)subscriberId.SubscriberId;

            var result = mq.Subscribe(paramSubscribe);

            RunFinalizeSql("021.SubscribeToChannel_InvalidChannelId", _conn);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Errors);
        }

        [Test]
        public void SubscribeToChannel_InvalidSubscriberId()
        {
            RunInitSql("022.SubscribeToChannel_InvalidSubscriberId", _conn);

            IMessageQueue mq = CreateMQ();

            // getting channel ID// getting channel ID
            IMQGetChannelIdParams paramsGetChannelId = mq.CreateGetChannelIdParams();
            paramsGetChannelId.ChannelName = ConfigurationManager.AppSettings["ChannelName"];

            IMQGetChannelIdResult channelId = mq.GetChannelId(paramsGetChannelId);

            // getting subscriber ID
            long invalidSubscriberId = 666000666;

            // subscribing
            IMQSubscribeParams paramSubscribe = mq.CreateSubscribeParams();
            paramSubscribe.ChannelId = (long)channelId.ChannelId;
            paramSubscribe.SubscriberId = invalidSubscriberId;

            var result = mq.Subscribe(paramSubscribe);

            RunFinalizeSql("022.SubscribeToChannel_InvalidSubscriberId", _conn);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Errors);
        }

        [Test]
        public void PushMessage_Success()
        {
            RunInitSql("030.PushMessage_Success", _conn);

            IMessageQueue mq = CreateMQ();

            // getting channel ID
            IMQGetChannelIdParams paramsGetChannelId = mq.CreateGetChannelIdParams();
            paramsGetChannelId.ChannelName = ConfigurationManager.AppSettings["ChannelName"];

            IMQGetChannelIdResult channelId = mq.GetChannelId(paramsGetChannelId);

            // getting subscribers ID
            IMQGetSubscriberIdParams paramsSenderId = mq.CreateGetSubscriberIdParams();
            paramsSenderId.SubscriberName = ConfigurationManager.AppSettings["SenderName"];
            IMQGetSubscriberIdResult senderId = mq.GetSubscriberId(paramsSenderId);

            IMQGetSubscriberIdParams paramsRecId = mq.CreateGetSubscriberIdParams();
            paramsSenderId.SubscriberName = ConfigurationManager.AppSettings["ReceiverName"];
            IMQGetSubscriberIdResult receiverId = mq.GetSubscriberId(paramsSenderId);

            // subscribing both
            IMQSubscribeParams paramSubscribe = mq.CreateSubscribeParams();
            paramSubscribe.ChannelId = (long)channelId.ChannelId;
            paramSubscribe.SubscriberId = (long)senderId.SubscriberId;
            var subscribeResult = mq.Subscribe(paramSubscribe);

            paramSubscribe.SubscriberId = (long)receiverId.SubscriberId;
            subscribeResult = mq.Subscribe(paramSubscribe);

            // sending message
            IMQPushMessageParams paramsPush = mq.CreatePushMessageParams();
            paramsPush.ChannelId = (long)channelId.ChannelId;
            paramsPush.SenderId = (long)senderId.SubscriberId;
            paramsPush.MessageType = ConfigurationManager.AppSettings["TestMessageType"];
            paramsPush.Payload = ConfigurationManager.AppSettings["TestMessagePayload"];
            paramsPush.RecipientId = null;

            var result = mq.PushMessage(paramsPush);

            RunFinalizeSql("030.PushMessage_Success", _conn);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Errors);
            Assert.Greater(result.MessageId, 0);
        }

        [Test]
        public void GetChannelMessages_Success()
        {
            RunInitSql("040.GetChannelMessages_Success", _conn);

            IMessageQueue mq = CreateMQ();

            // getting channel ID
            IMQGetChannelIdParams paramsGetChannelId = mq.CreateGetChannelIdParams();
            paramsGetChannelId.ChannelName = ConfigurationManager.AppSettings["ChannelName"];

            IMQGetChannelIdResult channelId = mq.GetChannelId(paramsGetChannelId);

            // getting subscribers ID
            IMQGetSubscriberIdParams paramsSenderId = mq.CreateGetSubscriberIdParams();
            paramsSenderId.SubscriberName = ConfigurationManager.AppSettings["SenderName"];
            IMQGetSubscriberIdResult senderId = mq.GetSubscriberId(paramsSenderId);

            IMQGetSubscriberIdParams paramsRecId = mq.CreateGetSubscriberIdParams();
            paramsRecId.SubscriberName = ConfigurationManager.AppSettings["ReceiverName"];
            IMQGetSubscriberIdResult receiverId = mq.GetSubscriberId(paramsRecId);

            // subscribing both
            IMQSubscribeParams paramSubscribe = mq.CreateSubscribeParams();
            paramSubscribe.ChannelId = (long)channelId.ChannelId;
            paramSubscribe.SubscriberId = (long)senderId.SubscriberId;
            var subscribeResult = mq.Subscribe(paramSubscribe);

            paramSubscribe.SubscriberId = (long)receiverId.SubscriberId;
            subscribeResult = mq.Subscribe(paramSubscribe);

            // sending 3 messages
            for (int i = 0; i < 3; ++i)
            {
                IMQPushMessageParams paramsPush = mq.CreatePushMessageParams();
                paramsPush.ChannelId = (long)channelId.ChannelId;
                paramsPush.SenderId = (long)senderId.SubscriberId;
                paramsPush.MessageType = ConfigurationManager.AppSettings["TestMessageType"];
                paramsPush.Payload = ConfigurationManager.AppSettings["TestMessagePayload"] + "-" + i;
                paramsPush.RecipientId = null;

                var pushRes = mq.PushMessage(paramsPush);
            }

            // getting messages
            IMQGetChannelMessagesParams paramsGetMsgs = mq.CreateGetChannelMessagesParams();
            paramsGetMsgs.ChannelId = (long)channelId.ChannelId;
            paramsGetMsgs.ReceiverId = (long)receiverId.SubscriberId;

            var result = mq.GetChannelMessages(paramsGetMsgs);

            RunFinalizeSql("040.GetChannelMessages_Success", _conn);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Errors);
            Assert.GreaterOrEqual(result.Messages.Count, 3);
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
