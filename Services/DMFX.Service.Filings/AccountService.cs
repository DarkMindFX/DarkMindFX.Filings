using DMFX.Interfaces;
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

namespace DMFX.Service.Filings
{
    public class AccountService : ServiceStack.Service
    {
        private IDal _dal = null;
        CompositionContainer _compContainer = null;



        public AccountService()
        {
            _dal = Global.Container.GetExport<IDal>().Value;
            _compContainer = Global.Container;
            InitDAL();
        }

        public object Any(CreateAccount request)
        {
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
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpected error: {0}", ex.Message) });
            }

            return response;
        }

        public object Any(InitSession request)
        {
            InitSessionResponse response = new InitSessionResponse();

            TransferHeader(request, response);

            try
            {
                string sessionId = Guid.NewGuid().ToString();

                SessionInfo sinfo = new SessionInfo();
                sinfo.AccountKey = request.AccountKey;
                sinfo.SessionStart = DateTime.Now;
                sinfo.SessionId = sessionId;

                // if current session exists - we are just using current session token
                SessionInfo existSession = _dal.GetSessionInfo(sinfo);
                if (existSession == null)
                {
                    _dal.InitSession(sinfo);

                    response.SessionToken = sessionId;                    
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpected error: {0}", ex.Message) });
            }

            return response;
        }

        public object Any(CloseSession request)
        {
            CloseSessionResponse response = new CloseSessionResponse();

            TransferHeader(request, response);
            try
            {
                
                SessionInfo sinfo = new SessionInfo();
                sinfo.SessionEnd = DateTime.Now;
                sinfo.SessionId = request.SessionToken;

                _dal.CloseSession(sinfo);

                response.Success = true;

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpected error: {0}", ex.Message) });
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
            Lazy<IDal> dal = _compContainer.GetExport<IDal>();
            IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionString", ConfigurationManager.AppSettings["ConnectionString"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }
        #endregion
    }
}