using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace DMFX.Service.Accounts
{
    public class AccountService : ServiceStack.Service
    {
        private Interfaces.DAL.IDal _dal = null;
        CompositionContainer _compContainer = null;
        private ILogger _logger = null;



        public AccountService()
        {
            _dal = Global.Container.GetExport<Interfaces.DAL.IDal>().Value;
            _logger = Global.Container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]).Value;

            _compContainer = Global.Container;
            InitDAL();
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
                    CreateUserAccountParams createParams = new CreateUserAccountParams();
                    createParams.Name = request.Name;
                    createParams.Email = request.Email;
                    createParams.AccountKey = EncodeUtils.CreateAccountKey();
                    createParams.PwdHash = EncodeUtils.GetPasswordHash(request.Pwd);

                    _dal.CreateUserAccount(createParams);

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
                string sessionId = Guid.NewGuid().ToString();

                Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();
                sinfo.AccountKey = request.AccountKey;
                sinfo.SessionStart = DateTime.Now;
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
                sinfo.SessionEnd = DateTime.Now;
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
                        sinfo.SessionStart = DateTime.Now;
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

                SessionInfo sessionInfo = _dal.GetSessionInfo(sessionParams, false);
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


        #region Support methods
        protected void TransferHeader(RequestBase request, ResponseBase response)
        {
            response.RequestID = request.RequestID;
            response.SessionToken = request.SessionToken;
        }

        private void InitDAL()
        {
            Lazy<Interfaces.DAL.IDal> dal = _compContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionString", ConfigurationManager.AppSettings["ConnectionString"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }


        #endregion
    }
}