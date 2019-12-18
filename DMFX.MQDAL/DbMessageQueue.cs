using DMFX.MQInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.MQDAL
{
    [Export("DBMQ", typeof(IMessageQueue))]
    public class DbMessageQueue : IMessageQueue
    {
        private IMQInitParams _params = null;
        private static string schema = "[dbo]";

        public void Init(IMQInitParams paramsInit)
        {
            _params = paramsInit;
        }

        public IMQCreateChannelResult CreateChannel(IMQCreateChannelParams paramsCreateChannel)
        {
            var result = new DbMQCreateChannelResult();

            string spName = "[SP_Create_Channel]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelName = new SqlParameter("@Channel_Name",
                                                            SqlDbType.NVarChar, 255,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsCreateChannel.ChannelName);

                SqlParameter paramMessageTimeout = new SqlParameter("@Message_Timeout",
                                                            SqlDbType.Time, 7,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsCreateChannel.MessageTimeout);

                SqlParameter paramOutChannelId = new SqlParameter("@Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current,
                                                            result.ChannelId);

                cmd.Parameters.Add(paramChannelName);
                cmd.Parameters.Add(paramMessageTimeout);
                cmd.Parameters.Add(paramOutChannelId);

                cmd.ExecuteNonQuery();

                result.ChannelId = (long)paramOutChannelId.Value;
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQGetChannelIdResult GetChannelId(IMQGetChannelIdParams paramsGetChnlId)
        {
            var result = new DbMQGetChannelIdResult();

            string spName = "[SP_Get_Channel_Id]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelName = new SqlParameter("@Channel_Name",
                                                            SqlDbType.NVarChar, 255,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsGetChnlId.ChannelName);

                SqlParameter paramOutChannelId = new SqlParameter("@Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current,
                                                            result.ChannelId);

                cmd.Parameters.Add(paramChannelName);
                cmd.Parameters.Add(paramOutChannelId);

                cmd.ExecuteNonQuery();

                result.ChannelId = (long)paramOutChannelId.Value;
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQGetChannelMessagesResult GetChannelMessages(IMQGetChannelMessagesParams paramsGetMsgs)
        {
            var result = new DbMQGetChannelMessagesResult();

            string spName = "[SP_Get_Channel_Messages]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelId = new SqlParameter("@IN_Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsGetMsgs.ChannelId);

                SqlParameter paramReceiverId = new SqlParameter("@IN_Receiver_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsGetMsgs.ReceiverId);

                SqlParameter paramMessageStatusId = new SqlParameter("@IN_Message_Status_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            (long)paramsGetMsgs.MessageStatus);

                cmd.Parameters.Add(paramChannelId);
                cmd.Parameters.Add(paramReceiverId);
                cmd.Parameters.Add(paramMessageStatusId);

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds);

                if (ds.Tables.Count >= 1)
                {
                }

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQPushMessageResult PushMessage(IMQPushMessageParams paramsPushMsg)
        {
            var result = new DbMQPushMessageResult();

            string spName = "[SP_Push_Message]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                //TODO: add parameters and calls here
                SqlParameter paramChannelId = new SqlParameter("@IN_Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsPushMsg.ChannelId);

                SqlParameter paramSenderId = new SqlParameter("@IN_Sender_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsPushMsg.SenderId);

                SqlParameter paramRecipientId = new SqlParameter("@IN_Recipient_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Current,
                                                            paramsPushMsg.RecipientId != null ? (object)paramsPushMsg.RecipientId : DBNull.Value);

                SqlParameter paramPayload = new SqlParameter("@IN_Payload",
                                                            SqlDbType.NVarChar, -1,
                                                            ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Current,
                                                            paramsPushMsg.Payload);

                SqlParameter paramOutMsgId = new SqlParameter("@OUT_Message_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current,
                                                            result.MessageId);


                cmd.Parameters.Add(paramChannelId);
                cmd.Parameters.Add(paramSenderId);
                cmd.Parameters.Add(paramRecipientId);
                cmd.Parameters.Add(paramPayload);
                cmd.Parameters.Add(paramOutMsgId);

                cmd.ExecuteNonQuery();

                result.MessageId = (long)paramOutMsgId.Value;
                result.Success = true;


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQRegisterSubscriberResult RegisterSubscriber(IMQRegisterSubscriberParams paramsRegSubscr)
        {
            var result = new DbMQRegisterSubscriberResult();

            string spName = "[SP_Register_Subscriber]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramSubsName = new SqlParameter("@Subscriber_Name",
                                                            SqlDbType.NVarChar, 255,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsRegSubscr.SubscriberName);

                SqlParameter paramSubsUrl = new SqlParameter("@Subscriber_Url",
                                                            SqlDbType.NVarChar, 255,
                                                            ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Current,
                                                            paramsRegSubscr.SubscriberUrl != null ? (object)paramsRegSubscr.SubscriberUrl : DBNull.Value);

                SqlParameter paramOutSubsId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current,
                                                            result.SubscriberId);

                cmd.Parameters.Add(paramSubsName);
                cmd.Parameters.Add(paramSubsUrl);
                cmd.Parameters.Add(paramOutSubsId);

                cmd.ExecuteNonQuery();

                result.SubscriberId = (long)paramOutSubsId.Value;
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQSetChannelSubscriptionStatusResult SetChannelSubscriptionStatus(IMQSetChannelSubscriptionStatusParams paramsSetSubscrStatus)
        {
            var result = new DbMQSetChannelSubscriptionStatusResult();

            string spName = "[SP_Set_Channel_Subscription_Status]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelId = new SqlParameter("@Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsSetSubscrStatus.ChannelId);

                SqlParameter paramSubscriberId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsSetSubscrStatus.SubscriberId);

                SqlParameter paramStatusId = new SqlParameter("@Status_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            (long)paramsSetSubscrStatus.Status);

                cmd.Parameters.Add(paramChannelId);
                cmd.Parameters.Add(paramSubscriberId);
                cmd.Parameters.Add(paramStatusId);
                cmd.ExecuteNonQuery();


                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQSetMessageStateResult SetMessageState(IMQSetMessageStateParams paramsSetMSgState)
        {
            var result = new DbMQSetMessageStateResult();

            string spName = "[SP_Set_Message_Routing_Status]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramMessageId = new SqlParameter("@Message_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsSetMSgState.MessageId);

                SqlParameter paramSubscriberId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsSetMSgState.SubscriberId);

                SqlParameter paramStatusId = new SqlParameter("@Status_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            (long)paramsSetMSgState.Status);

                cmd.Parameters.Add(paramMessageId);
                cmd.Parameters.Add(paramSubscriberId);
                cmd.Parameters.Add(paramStatusId);
                cmd.ExecuteNonQuery();


                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQSubscribeResult Subscribe(IMQSubscribeParams paramsSubscribe)
        {
            var result = new DbMQSubscribeResult();

            string spName = "[SP_Subscribe_To_Channel]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelId = new SqlParameter("@Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsSubscribe.ChannelId);

                SqlParameter paramSubscriberId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsSubscribe.SubscriberId);

 
                cmd.Parameters.Add(paramChannelId);
                cmd.Parameters.Add(paramSubscriberId);
                cmd.ExecuteNonQuery();

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQUnsubscribeResult Unsubscribe(IMQUnsubscribeParams paramsUnsubscribe)
        {
            var result = new DbMQUnsubscribeResult();

            string spName = "[SP_Unsubscribe_From_Channel]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelId = new SqlParameter("@Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsUnsubscribe.ChannelId);

                SqlParameter paramSubscriberId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsUnsubscribe.SubscriberId);


                cmd.Parameters.Add(paramChannelId);
                cmd.Parameters.Add(paramSubscriberId);
                cmd.ExecuteNonQuery();

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQRemoveSubscriberResult RemoveSubscriber(IMQRemoveSubscriberParams paramRemSubscr)
        {
            var result = new DbMQRemoveSubscriberResult();

            string spName = "[SP_Remove_Subscriber]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramSubscriberId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramRemSubscr.SubscriberId);


                cmd.Parameters.Add(paramSubscriberId);
                cmd.ExecuteNonQuery();

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
        }

        public IMQDeleteChannelResult DeleteChannel(IMQDeleteChannelParams paramsDelChannel)
        {
            var result = new DbMQDeleteChannelResult();

            string spName = "[SP_Delete_Channel]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramChannelId = new SqlParameter("@Channel_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsDelChannel.ChannelId);


                cmd.Parameters.Add(paramChannelId);
                cmd.ExecuteNonQuery();

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Message = ex.Message,
                    Code = Interfaces.EErrorCodes.MQDbError,
                    Type = Interfaces.EErrorType.Error
                });
            }

            return result;
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

        public IMQDeleteChannelParams CreateDeleteChannelParams()
        {
            return new DbMQDeleteChannelParams();
        }

        public IMQRemoveSubscriberParams CreateRemoveSubscriberParams()
        {
            return new DbMQRemoveSubscriberParams();
        }
        #endregion

        #region Support methods
        SqlConnection OpenConnection(string connName)
        {
            SqlConnection conn = new SqlConnection(_params.Params[connName]);
            conn.Open();

            return conn;
        }
        #endregion
    }
}
