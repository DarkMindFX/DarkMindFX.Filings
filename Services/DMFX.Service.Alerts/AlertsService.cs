using DMFX.Service.Common;
using DMFX.Service.DTO;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using DMFX.AlertsInterfaces;
using System;
using DMFX.Interfaces;

namespace DMFX.Service.Alerts
{
    public class AlertsService : ServiceBase
    {
        private IAlertsDal _dal = null;
        CompositionContainer _compContainer = null;

        public AlertsService()
        {
            _compContainer = Global.Container;
            InitDAL();
        }

        public object Any(GetAlertsTypes request)
        {
            GetAlertsTypesResponse response = new GetAlertsTypesResponse();

            TransferHeader(request, response);

            try
            {
                IAlertsDalGetAlertTypesParams getParams = _dal.CreateGetAlertTypesParams();

                var getResult = _dal.GetAlertTypes(getParams);

                if(getResult.Success)
                {
                    foreach(var at in getResult.Types)
                    {
                        response.Payload.Types.Add(new DTO.AlertType()
                        {
                            ID = at.ID,
                            Name = at.Name,
                            Desc = at.Desc
                        });
                    }

                    response.Success = true;

                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(getResult.Errors);
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.GeneralError,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return response;
        }

        public object Any(GetNotificationTypes request)
        {
            GetNotificationTypesResponse response = new GetNotificationTypesResponse();

            TransferHeader(request, response);

            try
            {
                IAlertsDalGetAlertNotificationTypesParams getParams = _dal.CreateGetAlertNotificationTypesParams();

                var getResult = _dal.GetAlertNotificationTypes(getParams);

                if (getResult.Success)
                {
                    foreach (var at in getResult.Types)
                    {
                        response.Payload.Types.Add(new DTO.NotificationType()
                        {
                            ID = at.ID,
                            Name = at.Value
                        });
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.GeneralError,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return response;
        }

        public object Any(GetAccountAlerts request)
        {
            GetAccountAlertsResponse response = new GetAccountAlertsResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IAlertsDalGetAccountSubscriptionsParams getSubsParams = _dal.CreateGetAccountSubscrParams();
                    getSubsParams.AccountKey = request.AccountKey;

                    var getSubsResult = _dal.GetAccountSubscriptions(getSubsParams);
                    if(getSubsResult.Success)
                    {
                        response.Payload.AccountKey = request.AccountKey;
                        foreach(var s in getSubsResult.Subscriptions)
                        {
                            AlertSubscription alertSub = new AlertSubscription()
                            {
                                AlertTypeId = s.TypeId,
                                ID = s.Id,
                                Name = s.Name,
                                NotificationTypeId = (long)s.NotificationTypeId,
                                Subscribed = (DateTime)s.SubscribedDttm,
                                StatusId = s.StatusId
                            };
                            foreach(var p in s.SubscriptionData)
                            {
                                alertSub.Properties.Add(new AlertSubscriptionProperty()
                                {
                                    Name = p.Key,
                                    Value = p.Value
                                });
                            }
                        }

                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.AddRange(getSubsResult.Errors);
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.GeneralError,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return response;
        }

        public object Any(AddAccountAlerts request)
        {
            AddAccountAlertsResponse response = new AddAccountAlertsResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IAlertsDalAddAccountSubscrParams addSubParams = _dal.CreateAddAccountSubscrParams();

                    foreach (var a in request.Alerts)
                    {
                        addSubParams.SubscriptonDetails = new Subscription()
                        {
                            AccountKey = request.AccountKey,
                            Name = a.Name,
                            NotificationTypeId = a.NotificationTypeId,
                            SubscribedDttm = DateTime.UtcNow,
                            TypeId = a.AlertTypeId
                        };
                        foreach(var p in a.Properties)
                        {
                            addSubParams.SubscriptonDetails.SubscriptionData.Add(p.Name, p.Value);
                        }
                        
                        var addSubResult = _dal.AddAlertSubscription(addSubParams);
                        if(addSubResult.Success)
                        {
                            response.Errors.Add(new Error()
                            {
                                Type = EErrorType.Info,
                                Code = EErrorCodes.Success,
                                Message = string.Format("Alert subscription {0} added", a.Name)
                            }); 
                        }
                        else
                        {
                            response.Errors.Add(new Error()
                            {
                                Type = EErrorType.Warning,
                                Code = EErrorCodes.AlertsSourceFail,
                                Message = string.Format("Failed to add alert subscription {0}", a.Name)
                            });
                        }
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.GeneralError,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return response;
        }

        public object Any(UpdateAccountAlerts request)
        {
            UpdateAccountAlertsResponse response = new UpdateAccountAlertsResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    IAlertsDalUpdateAccountSubscrParams updSubParams = _dal.CreateUpdateAccountSubscrParams();

                    foreach (var a in request.Alerts)
                    {
                        updSubParams.SubscriptonDetails = new Subscription()
                        {
                            AccountKey = request.AccountKey,
                            Name = a.Name,
                            NotificationTypeId = a.NotificationTypeId,
                            SubscribedDttm = DateTime.UtcNow,
                            TypeId = a.AlertTypeId
                        };
                        foreach (var p in a.Properties)
                        {
                            updSubParams.SubscriptonDetails.SubscriptionData.Add(p.Name, p.Value);
                        }

                        var updSubResult = _dal.UpdateAlertSubscription(updSubParams);
                        if (updSubResult.Success)
                        {
                            response.Errors.Add(new Error()
                            {
                                Type = EErrorType.Info,
                                Code = EErrorCodes.Success,
                                Message = string.Format("Alert subscription  {0}, {1} updated", a.ID, a.Name)
                            });
                        }
                        else
                        {
                            response.Errors.Add(new Error()
                            {
                                Type = EErrorType.Warning,
                                Code = EErrorCodes.AlertsSourceFail,
                                Message = string.Format("Failed to update alert subscription {0}, {1}", a.ID, a.Name)
                            });
                        }
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.GeneralError,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return response;
        }

        public object Any(RemoveAccountAlerts request)
        {
            RemoveAccountAlertsResponse response = new RemoveAccountAlertsResponse();

            TransferHeader(request, response);

            try
            {
                if (IsValidSessionToken(request))
                {
                    var remSubParams = _dal.CreateRemoveAccountSubscrParams();
                    
                    foreach(var s in request.SubscriptionIds)
                    {
                        remSubParams.SubscriptionIds.Add(s);
                    }

                    var remSeqResult = _dal.RemoveAlertSubscription(remSubParams);
                    if(remSeqResult.Success)
                    {
                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.AddRange(remSeqResult.Errors);
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Interfaces.Error()
                {
                    Code = Interfaces.EErrorCodes.GeneralError,
                    Type = Interfaces.EErrorType.Error,
                    Message = ex.Message
                });
            }

            return response;
        }

        #region Support methods

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return ValidateSession(request.SessionToken) == EErrorCodes.Success;
        }

        private EErrorCodes ValidateSession(string sessionToken)
        {
            EErrorCodes result = EErrorCodes.InvalidSession;

            if (sessionToken == ConfigurationManager.AppSettings["ServiceSessionToken"])
            {
                result = EErrorCodes.Success;
            }
            else
            {
                GetSessionInfo sinfo = new GetSessionInfo();
                sinfo.SessionToken = sessionToken;
                sinfo.CheckActive = true;

                DMFX.Client.Accounts.ServiceClient accnts = new Client.Accounts.ServiceClient();
                GetSessionInfoResponse sInfoResp = accnts.PostGetSessionInfo(sinfo);

                result = sInfoResp.Success ? EErrorCodes.Success : sInfoResp.Errors[0].Code;
            }

            return result;
        }

        private void InitDAL()
        {
            string dalType = ConfigurationManager.AppSettings["AlertsDalType"];
            _dal = _compContainer.GetExport<IAlertsDal>(dalType).Value;

            IAlertsDalInitParams initParams = _dal.CreateInitParams();
            initParams.Parameters["ConnectionStringAlerts"] = ConfigurationManager.AppSettings["ConnectionStringAlerts"];
            _dal.Init(initParams);

        }
        #endregion
    }
}