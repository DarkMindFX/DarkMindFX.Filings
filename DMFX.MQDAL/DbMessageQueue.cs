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

            conn.Close();

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

            conn.Close();

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

                cmd.Parameters.Add(paramChannelId);
                cmd.Parameters.Add(paramReceiverId);
  
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds);

                if (ds.Tables.Count >= 1)
                {
                    for(int i = 0; i < ds.Tables[0].Rows.Count; ++i)
                    {
                        DataRow r = ds.Tables[0].Rows[i];
                        MessageDetails md = new MessageDetails();
                        md.Id = (long)r["Message_Id"];
                        md.ChannelName = (string)r["Channel_Name"];
                        md.ChannelId = (long)r["Channel_Id"];
                        md.Payload = (string)r["Message_Payload"];
                        md.SenderId = !DBNull.Value.Equals(r["Sender_Id"]) ? (long?)r["Sender_Id"] : null;
                        md.SubscriberId = !DBNull.Value.Equals(r["Subscriber_Id"]) ? (long?)r["Subscriber_Id"] : null;
                        md.Type = (string)r["Message_Type"];
                        

                        result.Messages.Add(md);
                    }
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

            conn.Close();

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

                SqlParameter paramMsgType = new SqlParameter("@IN_Message_Type",
                                                            SqlDbType.NVarChar, 255,
                                                            ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Current,
                                                            paramsPushMsg.MessageType);

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
                cmd.Parameters.Add(paramMsgType);
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

            conn.Close();

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

            conn.Close();

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

            conn.Close();

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

            conn.Close();

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

            conn.Close();

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

            conn.Close();

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

            conn.Close();

            return result;
        }

        public IMQGetSubscriberIdResult GetSubscriberId(IMQGetSubscriberIdParams paramsGetChnlId)
        {
            var result = new DbMQGetSubscriberIdResult();

            string spName = "[SP_Get_Subscriber_Id]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramSubscriberName = new SqlParameter("@Subscriber_Name",
                                                            SqlDbType.NVarChar, 255,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsGetChnlId.SubscriberName);

                SqlParameter paramOutSubscriberId = new SqlParameter("@Subscriber_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Current,
                                                            result.SubscriberId);

                cmd.Parameters.Add(paramSubscriberName);
                cmd.Parameters.Add(paramOutSubscriberId);

                cmd.ExecuteNonQuery();

                result.SubscriberId = (long)paramOutSubscriberId.Value;
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

            conn.Close();

            return result;
        }

        public IMQDeleteMessageResult DeleteMessage(IMQDeleteMessageParams paramsDelMessage)
        {
            var result = new DbMQDeleteMessageResult();

            string spName = "[SP_Delete_Message]";
            SqlConnection conn = OpenConnection("ConnectionString");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                SqlParameter paramMessageId = new SqlParameter("@IN_Message_Id",
                                                            SqlDbType.BigInt, 0,
                                                            ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Current,
                                                            paramsDelMessage.MessageId);


                cmd.Parameters.Add(paramMessageId);
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

            conn.Close();

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

        public IMQGetSubscriberIdParams CreateGetSubscriberIdParams()
        {
            return new DbMQGetSubscriberIdParams();
        }

        public IMQGetChannelMessagesParams CreateGetChannelMessagesParams()
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

        public IMQDeleteMessageParams CreateDeleteMessageParams()
        {
            return new DbMQDeleteMessageParams();
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
