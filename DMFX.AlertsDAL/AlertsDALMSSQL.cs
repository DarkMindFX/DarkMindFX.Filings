using DMFX.AlertsInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.AlertsDAL
{
    [Export("MSSQL", typeof(IAlertsDal))]
    public class AlertsDALMSSQL : IAlertsDal
    {
        private object _lockConn = new object();
        private static string schema = "[dbo]";
        private static string connName = "ConnectionStringAlerts";
        IAlertsDalInitParams _dalParams = null;

        public void Init(IAlertsDalInitParams initParams)
        {
            _dalParams = initParams;
        }

        public IAlertsDalAddAccountSubscrResult AddAlertSubscription(IAlertsDalAddAccountSubscrParams addSubscrParams)
        {
            IAlertsDalAddAccountSubscrResult result = new MSSQL.AlertsDalAddAccountSubscrResult();

            string spName = "[SP_Add_Alert_Subscription]";

            SqlConnection conn = OpenConnection(connName);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.AlertsSourceFail,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return result;
        }

        public IAlertsDalGetAccountSubscriptionsResult GetAccountSubscriptions(IAlertsDalGetAccountSubscriptionsParams getSubsParams)
        {
            IAlertsDalGetAccountSubscriptionsResult result = new MSSQL.AlertsDalGetAccountSubscriptionsResult();

            string spName = "[SP_Get_Account_Subscriptions]";

            SqlConnection conn = OpenConnection(connName);

            SqlParameter paramAccountKey = new SqlParameter("@SubscriberAccountKey", SqlDbType.NVarChar, 255, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Current, getSubsParams.AccountKey);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            cmd.Parameters.Add(paramAccountKey);

            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Subscription subs = new Subscription();
                        subs.Id = (long)r["Alert_Subscription_Id"];
                        subs.Name = !DBNull.Value.Equals(r["Alert_Subscription_Name"]) ? (string)r["Alert_Subscription_Name"] : null;
                        subs.AccountKey = !DBNull.Value.Equals(r["Subscriber_Account_Key"]) ? (string)r["Subscriber_Account_Key"] : null;
                        subs.TypeId = (long)r["Alert_Type_Id"];
                        subs.TypeName = !DBNull.Value.Equals(r["Alert_Type_Name"]) ? (string)r["Alert_Type_Name"] : null;
                        subs.NotificationTypeId = (long)r["Notification_Type_Id"];
                        subs.NotificationTypeName = !DBNull.Value.Equals(r["Alert_Notification_Type_Value"]) ? (string)r["Alert_Notification_Type_Value"] : null;
                        subs.SubscribedDttm = (DateTime)r["Subscribed_Dttm"];
                        subs.StatusId = (long)r["Alert_Subscription_Status_Id"];
                        subs.StatusName = !DBNull.Value.Equals(r["Alert_Subscription_Status_Value"]) ? (string)r["Alert_Subscription_Status_Value"] : null;

                        if (ds.Tables.Count > 1)
                        {
                            // from second table reading values for this subscription
                            foreach (var v in ds.Tables[1].Select(string.Format("Alert_Subscription_Id = {0}", subs.Id)))
                            {
                                subs.SubscriptionData.Add((string)v["Subscription_Property_Name"], (string)v["Subscription_Property_Value"]);
                            }
                        }

                        result.Subscriptions.Add(subs);
                    }

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.AlertsSourceFail,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return result;
        }

        public IAlertsDalGetAlertNotificationTypesResult GetAlertNotificationTypes(IAlertsDalGetAlertNotificationTypesParams getParams)
        {
            IAlertsDalGetAlertNotificationTypesResult result = new MSSQL.AlertsDalGetAlertNotificationTypesResult();

            string spName = "[SP_Get_Alert_Notification_Types]";

            SqlConnection conn = OpenConnection(connName);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        AlertNotificationType type = new AlertNotificationType();
                        type.ID = (long)r["Alert_Notification_Type_Id"];
                        type.Value = !DBNull.Value.Equals(r["Alert_Notification_Type_Value"]) ? (string)r["Alert_Notification_Type_Value"] : null;

                        result.Types.Add(type);
                    }

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.AlertsSourceFail,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return result;
        }

        public IAlertsDalGetAlertTypesResult GetAlertTypes(IAlertsDalGetAlertTypesParams getParams)
        {
            IAlertsDalGetAlertTypesResult result = new MSSQL.AlertsDalGetAlertTypesResult();

            string spName = "[SP_Get_Alert_Types]";

            SqlConnection conn = OpenConnection(connName);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        AlertType type = new AlertType();
                        type.ID = (long)r["Alert_Type_Id"];
                        type.Name = !DBNull.Value.Equals(r["Alert_Type_Name"]) ? (string)r["Alert_Type_Name"] : null;
                        type.Desc = !DBNull.Value.Equals(r["Alert_Type_Desc"]) ? (string)r["Alert_Type_Desc"] : null;

                        result.Types.Add(type);
                    }

                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.AlertsSourceFail,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return result;
        }

        public IAlertsDalUpdateAccountSubscrResult UpdateAlertSubscription(IAlertsDalUpdateAccountSubscrParams updSubscrParams)
        {
            IAlertsDalUpdateAccountSubscrResult result = new MSSQL.AlertsDalUpdateAccountSubscrResult();

            string spName = "[SP_Update_Alert_Subscription]";

            SqlConnection conn = OpenConnection(connName);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = schema + "." + spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            try
            {

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.AlertsSourceFail,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return result;
        }

        #region Create* methods for parameters
        public IAlertsDalAddAccountSubscrParams CreateAddAccountSubscrParams()
        {
            return new MSSQL.AlertsDalAddAccountSubscrParams();
        }

        public IAlertsDalGetAccountSubscriptionsParams CreateGetAccountSubscrParams()
        {
            return new MSSQL.AlertDalGetAccountSubscriptionsParams();
        }

        public IAlertsDalGetAlertNotificationTypesParams CreateGetAlertNotificationTypesParams()
        {
            return new MSSQL.AlertsDalGetAlertNotificationTypesParams();
        }

        public IAlertsDalGetAlertTypesParams CreateGetAlertTypesParams()
        {
            return new MSSQL.AlertsDalGetAlertTypesParams();
        }

        public IAlertsDalInitParams CreateInitParams()
        {
            return new MSSQL.AlertsDalInitParams();
        }

        public IAlertsDalRemoveAccountSubscrParams CreateRemoveAccountSubscrParams()
        {
            return new MSSQL.AlertsDalRemoveAccountSubscrParams();
        }

        public IAlertsDalUpdateAccountSubscrParams CreatUpdateAccountSubscrParams()
        {
            return new MSSQL.AlertsDalUpdateAccountSubscrParams();
        }
        #endregion

        #region Support methods
        private SqlConnection OpenConnection(string name)
        {
            SqlConnection conn = new SqlConnection(_dalParams.Parameters[name]);
            conn.Open();

            return conn;
        }
        #endregion
    }
}
