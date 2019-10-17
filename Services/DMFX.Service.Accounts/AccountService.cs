using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;


namespace DMFX.Service.Accounts
{
    public class AccountService : ServiceBase
    {
        private Interfaces.DAL.IDal _dal = null;
        CompositionContainer _compContainer = null;


        public AccountService()
        {
            _dal = Global.Container.GetExport<Interfaces.DAL.IDal>().Value;
            _compContainer = Global.Container;
            InitDAL();
        }
        
        public object Any(GetSessionInfo request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetSessionInfo");
            GetSessionInfoResponse response = new GetSessionInfoResponse();
            TransferHeader(request, response);

            try
            {
                Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();

                sinfo.SessionId = !string.IsNullOrEmpty(request.SessionToken) ? request.SessionToken : string.Empty;

                sinfo = _dal.GetSessionInfo(sinfo, request.CheckActive);
                if (sinfo != null)
                {
                    response.SessionStart = sinfo.SessionStart;
                    response.SessionEnd = sinfo.SessionEnd;
                    if (response.SessionEnd > DateTime.MinValue)
                    {
                        response.Errors.Add(new Error() { Code = EErrorCodes.SessionClosed, Message = "Session with given token was closed", Type = EErrorType.Warning });
                    }
                    response.Success = true;

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

        public object Any(CreateAccount request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: CreateAccount");
            CreateAccountResponse response = new CreateAccountResponse();
            TransferHeader(request, response);

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

                    response.AccountKey = createParams.AccountKey;
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

        public object Any(InitSession request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: InitSession");
            InitSessionResponse response = new InitSessionResponse();

            TransferHeader(request, response);

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

        public object Any(CloseSession request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: CloseSession");
            CloseSessionResponse response = new CloseSessionResponse();

            TransferHeader(request, response);
            try
            {
                Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();
                sinfo.SessionEnd = DateTime.UtcNow;
                sinfo.SessionId = request.SessionToken;

                _dal.CloseSession(sinfo);

                response.Success = true;

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

        public object Any(Login request)
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

        public object Any(GetAccountInfo request)
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
                        response.AccountKey = accInfoParams.AccountKey;
                        response.Email = accResult.Email;
                        response.Name = accResult.Name;
                        response.DateExpires = accResult.DateExpires;
                        response.DateCreated = accResult.DateCreated;
                        response.DateExpiresStr = accResult.DateExpires.ToString();
                        response.DateCreatedStr = accResult.DateCreated.ToString();

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

        public object Any(ActivateAccount request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ActivateAccount");
            ActivateAccountResponse response = new ActivateAccountResponse();
            TransferHeader(request, response);
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

        public object Any(UpdateAccount request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: UpdateAccount");
            UpdateAccountResponse response = new UpdateAccountResponse();
            TransferHeader(request, response);
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

        public object Any(ChangePassword request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ChangePassword");
            UpdateAccountResponse response = new UpdateAccountResponse();
            TransferHeader(request, response);
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

        public object Any(ResetPassword request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ChangePassword");
            UpdateAccountResponse response = new UpdateAccountResponse();
            TransferHeader(request, response);
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

    
        #region Support methods

        private void InitDAL()
        {
            _logger.Log(EErrorType.Info, string.Format("InitDAL: Connecting to '{0}'", ConfigurationManager.AppSettings["ConnectionStringAccounts"]));

            Lazy<Interfaces.DAL.IDal> dal = _compContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionStringFilings", ConfigurationManager.AppSettings["ConnectionStringFilings"]);
            dalParams.Parameters.Add("ConnectionStringAccounts", ConfigurationManager.AppSettings["ConnectionStringAccounts"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
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

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }


        #endregion
        
    }
}