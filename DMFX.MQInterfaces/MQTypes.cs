using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.MQInterfaces
{
    public enum EMessageStatus
    {
        Completed = 1,
        Processing = 2,
        Waiting = 3
    }

    public enum EChannelSubscriptionStatus
    {
        Enabled = 1,
        Disabled = 2
    }

    public class MessageDetails
    {
        public long Id
        {
            get;
            set;
        }

        public long ChannelId
        {
            get;
            set;
        }

        public string ChannelName
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public string Payload
        {
            get;
            set;
        }

        public long? SenderId
        {
            get;
            set;
        }

        public long? SubscriberId
        {
            get;
            set;
        }

        public EMessageStatus MessageStatus
        {
            get;
            set;
        }
    }
}
