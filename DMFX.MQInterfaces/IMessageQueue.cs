using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.MQInterfaces
{
    public interface IMQInitParams
    {
        IDictionary<string, string> Params
        {
            get;
            set;
        }
    }

    public interface IMQPushMessageParams
    {
        long ChannelId
        {
            get;
            set;
        }

        string MessageType
        {
            get;
            set;
        }

        long SenderId
        {
            get;
            set;
        }

        long? RecipientId
        {
            get;
            set;
        }

        string Payload
        {
            get;
            set;
        }
    }

    public interface IMQPushMessageResult : IResult
    {
        long MessageId
        {
            get;
            set;
        }
    }

    public interface IMQRegisterSubscriberParams
    {
        string SubscriberName
        {
            get;
            set;
        }

        string SubscriberUrl
        {
            get;
            set;
        }
    }

    public interface IMQRegisterSubscriberResult : IResult
    {
        long SubscriberId
        {
            get;
            set;
        }
    }

    public interface IMQSubscribeParams
    {
        long SubscriberId
        {
            get;
            set;
        }

        long ChannelId
        {
            get;
            set;
        }
    }

    public interface IMQSubscribeResult : IResult
    {
    }

    public interface IMQUnsubscribeParams
    {
        long SubscriberId
        {
            get;
            set;
        }

        long ChannelId
        {
            get;
            set;
        }
    }

    public interface IMQUnsubscribeResult : IResult
    {
    }

    public interface IMQCreateChannelParams
    {
        string ChannelName
        {
            get;
            set;
        }

        TimeSpan MessageTimeout
        {
            get;
            set;
        }
    }

    public interface IMQCreateChannelResult : IResult
    {
        long ChannelId
        {
            get;
            set;
        }

    }

    public interface IMQGetChannelMessagesParams
    {
        long ReceiverId
        {
            get;
            set;
        }

        long ChannelId
        {
            get;
            set;
        }

        EMessageStatus? MessageStatus
        {
            get;
            set;
        }
    }

    public interface IMQGetChannelMessagesResult : IResult
    {
        IList<MessageDetails> Messages
        {
            get;
            set;
        }

    }

    public interface IMQSetMessageStateParams
    {
        long MessageId
        {
            get;
            set;
        }

        long SubscriberId
        {
            get;
            set;
        }

        EMessageStatus Status
        {
            get;
            set;
        }
    }

    public interface IMQSetMessageStateResult : IResult
    {
    }

    public interface IMQSetChannelSubscriptionStatusParams
    {
        long SubscriberId
        {
            get;
            set;
        }

        long ChannelId
        {
            get;
            set;
        }

        EChannelSubscriptionStatus Status
        {
            get;
            set;
        }
    }

    public interface IMQSetChannelSubscriptionStatusResult : IResult
    {
    }

    public interface IMQGetChannelIdParams
    {
        string ChannelName
        {
            get;
            set;
        }
    }

    public interface IMQGetChannelIdResult : IResult
    {
        long? ChannelId
        {
            get;
            set;
        }
    }

    public interface IMQDeleteChannelParams
    {
        long ChannelId
        {
            get;
            set;
        }
    }

    public interface IMQDeleteChannelResult : IResult
    {
    }

    public interface IMQRemoveSubscriberParams
    {
        long SubscriberId
        {
            get;
            set;
        }
    }

    public interface IMQRemoveSubscriberResult : IResult
    {
    }

    public interface IMQGetSubscriberIdParams
    {
        string SubscriberName
        {
            get;
            set;
        }
    }

    public interface IMQGetSubscriberIdResult : IResult
    {
        long? SubscriberId
        {
            get;
            set;
        }
    }

    public interface IMessageQueue
    {
        void Init(IMQInitParams paramsInit);

        IMQPushMessageResult PushMessage(IMQPushMessageParams paramsPushMsg);

        IMQRegisterSubscriberResult RegisterSubscriber(IMQRegisterSubscriberParams paramsRegSubscr);

        IMQCreateChannelResult CreateChannel(IMQCreateChannelParams paramsCreateChannel);

        IMQGetChannelMessagesResult GetChannelMessages(IMQGetChannelMessagesParams paramsGetMsgs);

        IMQSetMessageStateResult SetMessageState(IMQSetMessageStateParams paramsSetMSgState);

        IMQSetChannelSubscriptionStatusResult SetChannelSubscriptionStatus(IMQSetChannelSubscriptionStatusParams paramsSetSubscrStatus);

        IMQGetChannelIdResult GetChannelId(IMQGetChannelIdParams paramsGetChnlId);

        IMQSubscribeResult Subscribe(IMQSubscribeParams paramsSubscribe);

        IMQUnsubscribeResult Unsubscribe(IMQUnsubscribeParams paramsUnsubscribe);

        IMQRemoveSubscriberResult RemoveSubscriber(IMQRemoveSubscriberParams paramRemSubscr);

        IMQDeleteChannelResult DeleteChannel(IMQDeleteChannelParams paramsDelChannel);

        IMQGetSubscriberIdResult GetSubscriberId(IMQGetSubscriberIdParams paramsGetSubsId);

        // Create*Params methods
        IMQInitParams CreateInitParams();

        IMQPushMessageParams CreatePushMessageParams();

        IMQRegisterSubscriberParams CreateRegisterSubscriberParams();

        IMQCreateChannelParams CreateCreateChannelParams();

        IMQGetChannelMessagesParams CreateIGetChannelMessagesParams();

        IMQSetMessageStateParams CreateSetMessageStateParams();

        IMQSetChannelSubscriptionStatusParams CreateSetChannelSubscriptionStatusParams();

        IMQGetChannelIdParams CreateGetChannelIdParams();

        IMQSubscribeParams CreateSubscribeParams();

        IMQUnsubscribeParams CreateUnsubscribeParams();

        IMQDeleteChannelParams CreateDeleteChannelParams();

        IMQRemoveSubscriberParams CreateRemoveSubscriberParams();

        IMQGetSubscriberIdParams CreateGetSubscriberIdParams();


    }
}
