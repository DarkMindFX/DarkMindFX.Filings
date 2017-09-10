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
                    response.Errors.Add(new Error() { Code = EErrorCodes.UserAccountExists, Type = EErrorType.Error, Message = string.Empty });
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