using DMFX.Interfaces;
using DMFX.MQInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.MQDAL
{
    public class DbMQInitParams : IMQInitParams
    {
        public DbMQInitParams()
        {
            Params = new Dictionary<string, string>();
            Params["ConnectionString"] = null;
        }

        public IDictionary<string, string> Params
        {
            get;
            set;
        }
    }

    public class DbMQPushMessageParams : IMQPushMessageParams
    {
        public long ChannelId
        {
            get;
            set;
        }

        public string MessageType
        {
            get;
            set;
        }

        public long SenderId
        {
            get;
            set;
        }

        public long? RecipientId
        {
            get;
            set;
        }

        public string Payload
        {
            get;
            set;
        }
    }

    public class DbMQPushMessageResult : ResultBase, IMQPushMessageResult
    {
        public long MessageId
        {
            get;
            set;
        }
    }

    public class DbMQRegisterSubscriberParams : IMQRegisterSubscriberParams
    {
        public string SubscriberName
        {
            get;
            set;
        }

        public string SubscriberUrl
        {
            get;
            set;
        }
    }

    public class DbMQRegisterSubscriberResult : ResultBase, IMQRegisterSubscriberResult
    {
        public long SubscriberId
        {
            get;
            set;
        }
    }

    public class DbMQSubscribeParams : IMQSubscribeParams
    {
        public long SubscriberId
        {
            get;
            set;
        }

        public long ChannelId
        {
            get;
            set;
        }
    }

    public class DbMQSubscribeResult : ResultBase, IMQSubscribeResult
    {
    }

    public class DbMQUnsubscribeParams : IMQUnsubscribeParams
    {
        public long SubscriberId
        {
            get;
            set;
        }

        public long ChannelId
        {
            get;
            set;
        }
    }

    public class DbMQUnsubscribeResult : ResultBase, IMQUnsubscribeResult
    {
    }

    public class DbMQCreateChannelParams : IMQCreateChannelParams
    {
        public DbMQCreateChannelParams()
        {
            MessageTimeout = TimeSpan.FromMinutes(10);
        }

        public string ChannelName
        {
            get;
            set;
        }

        public TimeSpan MessageTimeout
        {
            get;
            set;
        }
    }

    public class DbMQCreateChannelResult : ResultBase, IMQCreateChannelResult
    {
        public long ChannelId
        {
            get;
            set;
        }

    }

    public class DbMQGetChannelMessagesParams : ResultBase, IMQGetChannelMessagesParams
    {
        public long ReceiverId
        {
            get;
            set;
        }

        public long ChannelId
        {
            get;
            set;
        }

        public EMessageStatus? MessageStatus
        {
            get;
            set;
        }
    }

    public class DbMQGetChannelMessagesResult : ResultBase, IMQGetChannelMessagesResult
    {
        public DbMQGetChannelMessagesResult()
        {
            Messages = new List<MessageDetails>();
        }

        public IList<MessageDetails> Messages
        {
            get;
            set;
        }

    }

    public class DbMQSetMessageStateParams : IMQSetMessageStateParams
    {
        public long MessageId
        {
            get;
            set;
        }

        public long SubscriberId
        {
            get;
            set;
        }

        public EMessageStatus Status
        {
            get;
            set;
        }
    }

    public class DbMQSetMessageStateResult : ResultBase, IMQSetMessageStateResult
    {
    }

    public class DbMQSetChannelSubscriptionStatusParams : IMQSetChannelSubscriptionStatusParams
    {
        public long SubscriberId
        {
            get;
            set;
        }

        public long ChannelId
        {
            get;
            set;
        }

        public EChannelSubscriptionStatus Status
        {
            get;
            set;
        }
    }

    public class DbMQSetChannelSubscriptionStatusResult : ResultBase, IMQSetChannelSubscriptionStatusResult
    {
    }

    public class DbMQGetChannelIdParams : IMQGetChannelIdParams
    {
        public string ChannelName
        {
            get;
            set;
        }
    }

    public class DbMQGetChannelIdResult : ResultBase, IMQGetChannelIdResult
    {
        public long? ChannelId
        {
            get;
            set;
        }
    }

    public class DbMQDeleteChannelParams : IMQDeleteChannelParams
    {
        public long ChannelId
        {
            get;
            set;
        }
    }

    public class DbMQDeleteChannelResult : ResultBase, IMQDeleteChannelResult
    {
    }

    public class DbMQRemoveSubscriberParams : IMQRemoveSubscriberParams
    {
        public long SubscriberId
        {
            get;
            set;
        }
    }

    public class DbMQRemoveSubscriberResult : ResultBase, IMQRemoveSubscriberResult
    {
    }

    public class DbMQGetSubscriberIdParams : IMQGetSubscriberIdParams
    {
        public string SubscriberName
        {
            get;
            set;
        }
    }

    public class DbMQGetSubscriberIdResult : ResultBase, IMQGetSubscriberIdResult
    {
        public long? SubscriberId
        {
            get;
            set;
        }
    }
}

