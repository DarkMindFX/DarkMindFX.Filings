using DMFX.MQInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMFX.MQClient
{
    public class NewChannelMessagesDelegateEventArgs
    {
        public NewChannelMessagesDelegateEventArgs(IList<MessageDetails> messages = null)
        {
            Messages = new List<MessageDetails>();
            if (messages != null)
            {
                (Messages as List<MessageDetails>).AddRange(messages);
            }
        }

        public IList<MessageDetails> Messages
        {
            get;
            set;
        }
    }

    public class Client : IDisposable
    {
        private IMessageQueue _mq = null;
        private string _name = null;
        private Dictionary<string, long> _channels = null;
        private long _subscriberId = 0;
        private Task _listener = null;
        private bool _isListening = false;
        private object _lockChannels = new object();

        public delegate void NewChannelMessagesDelegate(object sender, NewChannelMessagesDelegateEventArgs args);

        public Client(IMessageQueue mq)
        {
            _mq = mq;
            _channels = new Dictionary<string, long>();
            PollPeriod = 100;
            CleanProcessed = false;
        }

        public event NewChannelMessagesDelegate NewChannelMessages;

        public bool Init(string name)
        {
            if (_subscriberId <= 0 && _listener == null)
            {
                IMQRegisterSubscriberParams paramsRegSubs = _mq.CreateRegisterSubscriberParams();
                paramsRegSubs.SubscriberName = name;

                var result = _mq.RegisterSubscriber(paramsRegSubs);
                IMQGetSubscriberIdResult resGetId = null;
                if (!result.Success)
                {
                    IMQGetSubscriberIdParams paramGetId = _mq.CreateGetSubscriberIdParams();
                    paramGetId.SubscriberName = name;

                    resGetId = _mq.GetSubscriberId(paramGetId);
                    if (resGetId.Success)
                    {
                        _subscriberId = (long)resGetId.SubscriberId;
                    }
                }
                else
                {
                    _subscriberId = result.SubscriberId;

                }

                _name = name;

                _listener = new Task(ListenerThread);
                _isListening = true;
                _listener.Start();


                return result.Success || (resGetId != null && resGetId.Success);
            }
            else
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int PollPeriod
        {
            get;
            set;
        }

        public bool CleanProcessed
        {
            get;
            set;
        }

        public bool Subscribe(string channel)
        {
            ValidateInitialized();

            IMQCreateChannelParams paramsCreateChannelId = _mq.CreateCreateChannelParams();
            paramsCreateChannelId.ChannelName = channel;

            var resChannelId = _mq.CreateChannel(paramsCreateChannelId);

            if (resChannelId.Success)
            {
                long channelId = resChannelId.ChannelId;
                lock (_lockChannels)
                {
                    if (!_channels.ContainsKey(channel))
                    {
                        _channels.Add(channel, resChannelId.ChannelId);
                    }
                }

                IMQSubscribeParams paramsSubs = _mq.CreateSubscribeParams();
                paramsSubs.ChannelId = channelId;
                paramsSubs.SubscriberId = _subscriberId;

                var resSubs = _mq.Subscribe(paramsSubs);

                return resSubs.Success;

            }
            else
            {
                return resChannelId.Success;
            }
        }

        public bool Unsubscribe(string channel = null)
        {
            ValidateInitialized();

            List<long> chnls = new List<long>();
            if (channel != null)
            {
                lock (_lockChannels)
                {
                    if (_channels.ContainsKey(channel))
                    {
                        chnls.Add(_channels[channel]);
                    }
                }
            }
            else
            {
                lock (_lockChannels)
                {
                    chnls.AddRange(_channels.Values);
                }
            }

            IMQUnsubscribeParams paramsUnsusb = _mq.CreateUnsubscribeParams();

            foreach (var c in chnls)
            {

                paramsUnsusb.ChannelId = c;
                paramsUnsusb.SubscriberId = _subscriberId;
                var resUnsubscr = _mq.Unsubscribe(paramsUnsusb);
                if (resUnsubscr.Success)
                {
                    lock (_lockChannels)
                    {
                        _channels.Remove(_channels.FirstOrDefault(x => x.Value == c).Key);
                    }
                }
            }

            return true;
        }

        public bool Push(string channel, string type, string payload, string recipient = null)
        {
            ValidateInitialized();

            long? recId = null;
            if (recipient != null)
            {
                IMQGetSubscriberIdParams paramsGetSubId = _mq.CreateGetSubscriberIdParams();
                paramsGetSubId.SubscriberName = recipient;

                var resRecId = _mq.GetSubscriberId(paramsGetSubId);
                if (resRecId.Success)
                {
                    recId = resRecId.SubscriberId;
                }
            }


            if (_channels.ContainsKey(channel))
            {
                IMQPushMessageParams paramsPush = _mq.CreatePushMessageParams();
                paramsPush.ChannelId = _channels[channel];
                paramsPush.SenderId = _subscriberId;
                paramsPush.MessageType = type;
                paramsPush.Payload = payload;
                paramsPush.RecipientId = recId;

                var resPush = _mq.PushMessage(paramsPush);

                return resPush.Success;
            }
            else
            {
                return false;
            }

        }

        public bool SetMessageStatus(long msgId, EMessageStatus status)
        {
            ValidateInitialized();

            IMQSetMessageStateParams paramsSetMsgState = _mq.CreateSetMessageStateParams();
            paramsSetMsgState.SubscriberId = _subscriberId;
            paramsSetMsgState.MessageId = msgId;
            paramsSetMsgState.Status = status;

            var resSetMsgState = _mq.SetMessageState(paramsSetMsgState);

            return resSetMsgState.Success;
        }

        public void Dispose()
        {
            if (_listener != null)
            {
                _isListening = false;
                _listener = null;
            }
        }

        private void ListenerThread()
        {
            IMQGetChannelMessagesParams paramsGetMsg = _mq.CreateGetChannelMessagesParams();
            paramsGetMsg.ReceiverId = _subscriberId;

            List<long> channels = new List<long>();

            while (_isListening)
            {
                // waiting for next polling
                Thread.Sleep(this.PollPeriod);

                // getting list of channels which currently subscribed to
                channels.Clear();

                lock (_lockChannels)
                {
                    channels.AddRange(_channels.Values);
                }

                // getting messages for each channel
                foreach (var c in channels)
                {
                    paramsGetMsg.ChannelId = c;
                    var resGetMsg = _mq.GetChannelMessages(paramsGetMsg);
                    if (resGetMsg.Success && resGetMsg.Messages.Count > 0)
                    {
                        OnNewChannelMessages(resGetMsg.Messages);
                    }
                }
            }
        }

        private void OnNewChannelMessages(IList<MessageDetails> messages)
        {
            if (NewChannelMessages != null)
            {
                NewChannelMessagesDelegateEventArgs args = new NewChannelMessagesDelegateEventArgs(messages);
                NewChannelMessages(this, args);
            }
        }

        private void ValidateInitialized()
        {
            if (_subscriberId <= 0)
            {
                throw new TypeInitializationException("Client not initialized. Call Init() method", null);
            }
        }


    }
}
