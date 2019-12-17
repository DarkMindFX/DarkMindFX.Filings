using DMFX.MQInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.MQDAL
{
    [Export("DBMQ", typeof(IMessageQueue))]
    public class DbMessageQueue : IMessageQueue
    {

        public IMQCreateChannelResult CreateChannel(IMQCreateChannelParams paramsCreateChannel)
        {
            throw new NotImplementedException();
        }

        public IMQGetChannelIdResult GetChannelId(IMQGetChannelIdParams paramsGetChnlId)
        {
            throw new NotImplementedException();
        }

        public IMQGetChannelMessagesParams GetChannelMessages(IMQGetChannelMessagesParams paramsGetMsgs)
        {
            throw new NotImplementedException();
        }

        public void Init(IMQInitParams paramsInit)
        {
            throw new NotImplementedException();
        }

        public IMQPushMessageResult PushMessage(IMQPushMessageParams paramsPushMsg)
        {
            throw new NotImplementedException();
        }

        public IMQRegisterSubscriberResult RegisterSubscriber(IMQRegisterSubscriberParams paramsRegSubscr)
        {
            throw new NotImplementedException();
        }

        public IMQSetChannelSubscriptionStatusResult SetChannelSubscriptionStatus(IMQSetChannelSubscriptionStatusParams paramsSetSubscrStatus)
        {
            throw new NotImplementedException();
        }

        public IMQSetMessageStateResult SetMessageState(IMQSetMessageStateParams paramsSetMSgState)
        {
            throw new NotImplementedException();
        }

        public IMQSubscribeResult Subscribe(IMQSubscribeParams paramsSubscribe)
        {
            throw new NotImplementedException();
        }

        public IMQUnsubscribeResult Unsubscribe(IMQUnsubscribeParams paramsUnsubscribe)
        {
            throw new NotImplementedException();
        }

        #region Create* methods
        

        public IMQCreateChannelParams CreateCreateChannelParams()
        {
            return new DbMQCreateChannelParams();
        }

        public IMQGetChannelIdParams CreateGetChannelIdParams()
        {
            return new DbMQGetChannelIdParams();
        }

        public IMQGetChannelMessagesParams CreateIGetChannelMessagesParams()
        {
            return new DbMQGetChannelMessagesParams();
        }

        public IMQInitParams CreateInitParams()
        {
            return new DbMQInitParams();
        }

        public IMQPushMessageParams CreatePushMessageParams()
        {
            return new DbMQPushMessageParams();
        }

        public IMQRegisterSubscriberParams CreateRegisterSubscriberParams()
        {
            return new DbMQRegisterSubscriberParams();
        }

        public IMQSetChannelSubscriptionStatusParams CreateSetChannelSubscriptionStatusParams()
        {
            return new DbMQSetChannelSubscriptionStatusParams();
        }

        public IMQSetMessageStateParams CreateSetMessageStateParams()
        {
            return new DbMQSetMessageStateParams();
        }

        public IMQSubscribeParams CreateSubscribeParams()
        {
            return new DbMQSubscribeParams();
        }

        public IMQUnsubscribeParams CreateUnsubscribeParams()
        {
            return new DbMQUnsubscribeParams();
        }
        #endregion
    }
}
