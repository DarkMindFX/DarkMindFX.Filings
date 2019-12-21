using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.MQInterfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using Newtonsoft.Json;

namespace DMFX.Service.Accounts
{
    public class AccountRequestsProcessor : IDisposable
    {
        private Interfaces.DAL.IDal _dal = null;
        CompositionContainer _compContainer = null;
        protected ILogger _logger = null;
        private IMessageQueue _mq = null;
        private DMFX.MQClient.Client _mqClient = null;
        private string _accountsChannel = null;


        public AccountRequestsProcessor()
        {
            _logger = Global.Logger;
            _dal = Global.Container.GetExport<Interfaces.DAL.IDal>().Value;
            _compContainer = Global.Container;

            InitDAL();
            InitMQClient();
        }

        public ResponseBase Any(GetSessionInfo request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetSessionInfo");
            GetSessionInfoResponse response = new GetSessionInfoResponse();
            
            try
            {
                Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();

                sinfo.SessionId = !string.IsNullOrEmpty(request.SessionToken) ? request.SessionToken : string.Empty;

                _logger.Log(EErrorType.Info, string.Format("_dal.GetSessionInfo({0}, {1})", sinfo.SessionId, request.CheckActive));

                sinfo = _dal.GetSessionInfo(sinfo, request.CheckActive);
                if (sinfo != null)
                {
                    response.Payload.SessionStart = sinfo.SessionStart;
                    response.Payload.SessionEnd = sinfo.SessionEnd;
                    if (!request.CheckActive)
                    {
                        // checking all sessions - just returning the warning that this session was closed
                        if (response.Payload.SessionEnd > DateTime.MinValue)
                        {
                            response.Errors.Add(new Error() { Code = EErrorCodes.SessionClosed, Message = "Session with given token was closed", Type = EErrorType.Warning });
                        }
                        response.Success = true;
                    }
                    else
                    {
                        // checking for active session!
                        // if expiration date >= now - closing the session and returning error; in other case returning true
                        _logger.Log(EErrorType.Info, string.Format("Expires: {0}, Now: {1}", sinfo.SessionExpires, DateTime.UtcNow));
                        if (sinfo.SessionExpires <= DateTime.UtcNow)
                        {
                            _logger.Log(EErrorType.Info, string.Format("Closing session {0}", sinfo.SessionId));
                            sinfo.SessionEnd = DateTime.UtcNow;
                            _dal.CloseSession(sinfo);

                            response.Errors.Add(new Error() { Code = EErrorCodes.SessionClosed, Message = "Session with given token expired and was closed", Type = EErrorType.Error });
                            response.Success = false;
                        }
                        else
                        {
                            response.Success = true;
                        }

                    }

                }
                else
                {
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Message = "Invalid session token", Type = EErrorType.Error });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unexpected error: {0}", ex.Message) });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: GetSessionInfo");

            return response;
        }

        public ResponseBase Any(CreateAccount request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: CreateAccount");
            CreateAccountResponse response = new CreateAccountResponse();

            try
            {
                GetUserAccountInfoParams accParams = new GetUserAccountInfoParams();
                accParams.Email = request.Email;

                GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accParams);
                if (accResult == null)
                {
                    // creating account
                    CreateUpdateUserAccountParams createParams = new CreateUpdateUserAccountParams();
                    createParams.Name = request.Name;
                    createParams.Email = request.Email;
                    createParams.AccountKey = EncodeUtils.CreateAccountKey();
                    createParams.PwdHash = EncodeUtils.GetPasswordHash(request.Pwd);
                    createParams.ActivationCode = EncodeUtils.CreateActivationCode();
                    createParams.State = "Pending"; // TODO; change to consts

                    _dal.CreateUserAccount(createParams);

                    SendMailResponse mailerResponse = SendAccountConfirmEmail(createParams.Email, createParams.AccountKey, createParams.Name);

                    response.Payload.AccountKey = createParams.AccountKey;
                    response.Success = true;

                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountExists, Type = EErrorType.Error, Message = "User with specified data already exists" });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unexpected error: {0}", ex.Message) });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: CreateAccount");

            return response;
        }

        public ResponseBase Any(InitSession request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: InitSession");
            InitSessionResponse response = new InitSessionResponse();

            try
            {
                // checking account key validity
                GetUserAccountInfoParams accParams = new GetUserAccountInfoParams();
                accParams.AccountKey = request.AccountKey;
                GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accParams);
                if (accResult != null)
                {
                    string sessionId = Guid.NewGuid().ToString();

                    Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();
                    sinfo.AccountKey = request.AccountKey;
                    sinfo.SessionStart = DateTime.UtcNow;
                    sinfo.SessionExpires = DateTime.UtcNow
                            + TimeSpan.FromMinutes(ConfigurationManager.AppSettings["SessionExpiresMins"] != null ? Int32.Parse(ConfigurationManager.AppSettings["SessionExpiresMins"]) : 60);
                    sinfo.SessionId = sessionId;

                    // if current session exists - we are just using current session token
                    Interfaces.DAL.SessionInfo existSession = _dal.GetSessionInfo(sinfo, true);
                    if (existSession == null)
                    {
                        _dal.InitSession(sinfo);

                        response.SessionToken = sessionId;
                    }
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountNotFound, Type = EErrorType.Error, Message = "Invalid account key provided" });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unexpected error: {0}", ex.Message) });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: InitSession");

            return response;
        }

        public ResponseBase Any(CloseSession request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: CloseSession");
            CloseSessionResponse response = new CloseSessionResponse();

            try
            {
                _logger.Log(EErrorType.Info, string.Format("Closing Session: {0}", request.SessionToken));
                Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();
                sinfo.SessionEnd = DateTime.UtcNow;
                sinfo.SessionId = request.SessionToken;

                _dal.CloseSession(sinfo);

                response.Success = true;

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unexpected error: {0}", ex.Message) });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: CloseSession");

            return response;
        }

        public ResponseBase Any(Login request)
        {
            LoginResponse response = new LoginResponse();
            _logger.Log(EErrorType.Info, " ****** Call start: Login");

            try
            {
                GetUserAccountInfoParams accParams = new GetUserAccountInfoParams();
                accParams.AccountKey = null;
                accParams.Email = request.Email;
                GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accParams);
                if (accResult != null)
                {
                    string pwdHash = EncodeUtils.GetPasswordHash(request.Pwd);
                    if (accResult.PwdHash == pwdHash)
                    {
                        string sessionId = Guid.NewGuid().ToString();

                        Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();
                        sinfo.AccountKey = accResult.AccountKey;
                        sinfo.SessionStart = DateTime.UtcNow;
                        sinfo.SessionExpires = DateTime.UtcNow
                            + TimeSpan.FromMinutes(ConfigurationManager.AppSettings["SessionExpiresMins"] != null ? Int32.Parse(ConfigurationManager.AppSettings["SessionExpiresMins"]) : 60);
                        sinfo.SessionId = sessionId;

                        _dal.InitSession(sinfo);

                        response.SessionToken = sessionId;

                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountNotFound, Type = EErrorType.Error, Message = "Email / password combination not found" });
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountNotFound, Type = EErrorType.Error, Message = "Account not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unexpected error: {0}", ex.Message) });
            }

            return response;
        }

        public ResponseBase Any(GetAccountInfo request)
        {
            GetAccountInfoResponse response = new GetAccountInfoResponse();

            try
            {
                SessionInfo sessionParams = new SessionInfo();
                sessionParams.SessionId = request.SessionToken;

                SessionInfo sessionInfo = _dal.GetSessionInfo(sessionParams, true);
                if (sessionInfo != null)
                {
                    // getting account details
                    GetUserAccountInfoParams accInfoParams = new GetUserAccountInfoParams();
                    accInfoParams.AccountKey = sessionInfo.AccountKey;

                    GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accInfoParams);
                    if (accResult != null)
                    {
                        response.Payload.AccountKey = accInfoParams.AccountKey;
                        response.Payload.Email = accResult.Email;
                        response.Payload.Name = accResult.Name;
                        response.Payload.DateExpires = accResult.DateExpires;
                        response.Payload.DateCreated = accResult.DateCreated;
                        response.Payload.DateExpiresStr = accResult.DateExpires.ToString();
                        response.Payload.DateCreatedStr = accResult.DateCreated.ToString();

                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountNotFound, Type = EErrorType.Error, Message = "No user account found for the given session" });
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session" });
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unexpected error: {0}", ex.Message)
                });
            }

            return response;
        }

        public ResponseBase Any(ActivateAccount request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ActivateAccount");
            ActivateAccountResponse response = new ActivateAccountResponse();

            try
            {
                GetUserAccountInfoParams accParams = new GetUserAccountInfoParams();
                accParams.Email = request.Email;

                GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accParams);
                if (accResult != null)
                {
                    if (accResult.ActivationCode == request.ActivationCode)
                    {
                        CreateUpdateUserAccountParams updateParams = new CreateUpdateUserAccountParams();
                        updateParams.AccountKey = accResult.AccountKey;
                        updateParams.State = "active"; // TODO: need to change to consts

                        _dal.UpdateUserAccount(updateParams);

                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error()
                        {
                            Code = EErrorCodes.UserAccountNotValidated,
                            Type = EErrorType.Error,
                            Message = "Invalid activation code provided - account was not activated"
                        }
                    );
                    }
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error()
                    {
                        Code = EErrorCodes.UserAccountNotFound,
                        Type = EErrorType.Error,
                        Message = "User account was not found."
                    }
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unexpected error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: ActivateAccount");

            return response;
        }

        public ResponseBase Any(UpdateAccount request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: UpdateAccount");
            UpdateAccountResponse response = new UpdateAccountResponse();

            try
            {
                SessionInfo sessionParams = new SessionInfo();
                sessionParams.SessionId = request.SessionToken;

                SessionInfo sessionInfo = _dal.GetSessionInfo(sessionParams, true);
                if (sessionInfo != null)
                {

                    CreateUpdateUserAccountParams updateParams = new CreateUpdateUserAccountParams();
                    updateParams.AccountKey = sessionInfo.AccountKey;
                    updateParams.Email = request.Email ?? null;
                    updateParams.Name = request.Name ?? null;
                    updateParams.PwdHash = !string.IsNullOrEmpty(request.Pwd) ? EncodeUtils.GetPasswordHash(request.Pwd) : null;
                    updateParams.State = request.State ?? null;

                    _dal.UpdateUserAccount(updateParams);

                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session" });
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unexpected error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: UpdateAccount");

            return response;
        }

        public ResponseBase Any(ChangePassword request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ChangePassword");
            UpdateAccountResponse response = new UpdateAccountResponse();

            try
            {
                SessionInfo sessionParams = new SessionInfo();
                sessionParams.SessionId = request.SessionToken;

                SessionInfo sessionInfo = _dal.GetSessionInfo(sessionParams, true);
                if (sessionInfo != null)
                {
                    // updating account details
                    CreateUpdateUserAccountParams updateParams = new CreateUpdateUserAccountParams();
                    updateParams.AccountKey = sessionInfo.AccountKey;
                    updateParams.Email = request.Email;
                    updateParams.PwdHash = EncodeUtils.GetPasswordHash(request.Pwd);

                    _dal.UpdateUserAccount(updateParams);

                    // getting account details
                    GetUserAccountInfoParams accInfoParams = new GetUserAccountInfoParams();
                    accInfoParams.AccountKey = sessionInfo.AccountKey;

                    GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accInfoParams);
                    if (accResult != null)
                    {
                        SendMailResponse mailerResponse = SendPasswordChangedNotificationEmail(updateParams.Email, accResult.Name);
                        if (!mailerResponse.Success)
                        {
                            response.Errors.Add(new Error()
                            {
                                Code = EErrorCodes.MailSendFailed,
                                Message = "Mail services returned errors. Check other errors",
                                Type = EErrorType.Warning
                            });
                            response.Errors.AddRange(mailerResponse.Errors);
                        }
                    }

                    response.Success = true;

                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session" });
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unexpected error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: ChangePassword");

            return response;
        }

        public ResponseBase Any(ResetPassword request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ChangePassword");
            UpdateAccountResponse response = new UpdateAccountResponse();

            try
            {
                // getting account details
                GetUserAccountInfoParams accInfoParams = new GetUserAccountInfoParams();
                accInfoParams.Email = request.Email;

                GetUserAccountInfoResult accResult = _dal.GetUserAccountInfo(accInfoParams);
                if (accResult != null && accResult.Success)
                {
                    string newPassword = EncodeUtils.GenerateRandomPassword();
                    // getting account details
                    CreateUpdateUserAccountParams updateParams = new CreateUpdateUserAccountParams();
                    updateParams.AccountKey = accResult.AccountKey;
                    updateParams.Email = request.Email;
                    updateParams.PwdHash = EncodeUtils.GetPasswordHash(newPassword);

                    _dal.UpdateUserAccount(updateParams);

                    SendMailResponse mailerResponse = SendPasswordResetNotificationEmail(updateParams.Email, accResult.Name, newPassword);
                    if (!mailerResponse.Success)
                    {
                        response.Errors.Add(new Error()
                        {
                            Code = EErrorCodes.MailSendFailed,
                            Message = "Mail services returned errors. Check other errors",
                            Type = EErrorType.Warning
                        });
                        response.Errors.AddRange(mailerResponse.Errors);
                    }

                    response.Success = true;


                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountNotFound, Type = EErrorType.Error, Message = "No account found for the given email" });
                }

            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unexpected error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: ChangePassword");

            return response;
        }

        public void Dispose()
        {
            if(_mqClient != null)
            {
                _mqClient.NewChannelMessages -= NewMessagesEventHandler;
                _mqClient.Unsubscribe(_accountsChannel);
                _mqClient.Dispose();
                _mqClient = null;
            }
        }

        #region Support methods

        private void InitDAL()
        {
            _logger.Log(EErrorType.Info, string.Format("InitDAL: Connecting to '{0}'", ConfigurationManager.AppSettings["ConnectionStringAccounts"]));

            Lazy<Interfaces.DAL.IDal> dal = _compContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionStringAccounts", ConfigurationManager.AppSettings["ConnectionStringAccounts"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }

        private void InitMQClient()
        {
            _accountsChannel = ConfigurationManager.AppSettings["MQAccountsChannelName"];
            _mq = Global.Container.GetExport<IMessageQueue>( ConfigurationManager.AppSettings["MessageQueueType"] ).Value;
            if(_mq != null)
            {
                IMQInitParams initParams = _mq.CreateInitParams();
                initParams.Params["ConnectionString"] = ConfigurationManager.AppSettings["ConnectionStringMsgBus"];

                _mq.Init(initParams);

                _mqClient = new MQClient.Client(_mq);
                if( _mqClient.Init(ConfigurationManager.AppSettings["MQSubscriberName"]) )
                {
                    if(_mqClient.Subscribe(_accountsChannel))
                    {
                        _mqClient.NewChannelMessages += NewMessagesEventHandler;
                    }
                }
            }
        }

        private void NewMessagesEventHandler(object sender, MQClient.NewChannelMessagesDelegateEventArgs args)
        {
            if (args != null && args.Messages != null)
            {
                foreach (var m in args.Messages)
                {
                    ResponseBase response = null; ;
                    string responseType = null;
                    switch(m.Type)
                    {
                        case "GetSessionInfo":
                            var request = JsonConvert.DeserializeObject<GetSessionInfo>(m.Payload);
                            response = Any(request);
                            response.RequestID = request.RequestID;
                            response.SessionToken = request.SessionToken;
                            responseType = "GetSessionInfoResponse";
                            break;                     

                    }

                    if(response != null && responseType != null)
                    {
                        string respPayload = JsonConvert.SerializeObject(response, Formatting.Indented);
                        _mqClient.Push(_accountsChannel, responseType, respPayload);
                        _mqClient.SetMessageStatus(m.Id, EMessageStatus.Completed);
                    }
                }
            }
        }

        private SendMailResponse SendAccountConfirmEmail(string email, string accountKey, string userName)
        {
            SendMailResponse result = null;
            MailDetails datails = new MailDetails();
            datails.MessageType = "AccountCreatedConfirmation";
            datails.ToAddress = email;
            datails.Parameters.Add("UserName", userName);
            datails.Parameters.Add("AccountKey", accountKey);
            datails.Parameters.Add("Login", email);

            SendMail sendMailRequest = new SendMail();
            sendMailRequest.SessionToken = ConfigurationManager.AppSettings["MailServiceSessionToken"];
            sendMailRequest.Details.Add(datails);

            Client.Mail.ServiceClient client = new Client.Mail.ServiceClient();
            result = client.PostSendMail(sendMailRequest);

            return result;
        }

        private SendMailResponse SendPasswordChangedNotificationEmail(string email, string userName)
        {
            SendMailResponse result = null;
            MailDetails datails = new MailDetails();
            datails.MessageType = "PasswordChangedConfirmation";
            datails.ToAddress = email;
            datails.Parameters.Add("UserName", userName);

            SendMail sendMailRequest = new SendMail();
            sendMailRequest.SessionToken = ConfigurationManager.AppSettings["MailServiceSessionToken"];
            sendMailRequest.Details.Add(datails);

            Client.Mail.ServiceClient client = new Client.Mail.ServiceClient();
            result = client.PostSendMail(sendMailRequest);

            return result;
        }

        private SendMailResponse SendPasswordResetNotificationEmail(string email, string userName, string newPassword)
        {
            SendMailResponse result = null;
            MailDetails datails = new MailDetails();
            datails.MessageType = "PasswordResetNotification";
            datails.ToAddress = email;
            datails.Parameters.Add("UserName", userName);
            datails.Parameters.Add("Password", newPassword);

            SendMail sendMailRequest = new SendMail();
            sendMailRequest.SessionToken = ConfigurationManager.AppSettings["MailServiceSessionToken"];
            sendMailRequest.Details.Add(datails);

            Client.Mail.ServiceClient client = new Client.Mail.ServiceClient();
            result = client.PostSendMail(sendMailRequest);

            return result;
        }

        protected bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }

        


        #endregion
    }
}