using DMFX.Interfaces;
using DMFX.Service.DTO;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.Common
{
    public abstract class ServiceBase : ServiceStack.Service
    {
        protected ILogger _logger = null;
        protected bool _logRequests = false;

        public ServiceBase()
        {
            _logger = GlobalBase.Logger;
        }

        public object Any(Echo request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: Echo");

            EchoResponse response = new EchoResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    _logger.Log(EErrorType.Info, string.Format("Echo message recieved/sent: {0}\t Session: {1}", request.Message, request.SessionToken));
                    response.Message = request.Message;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
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
                    Message = string.Format("Unpexcted error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: Echo");

            return response;
        }

        #region Support methods
        protected void TransferHeader(RequestBase request, ResponseBase response)
        {
            if (_logRequests)
            {
                LogRequest(request);
            }
            response.RequestID = request.RequestID;
            response.SessionToken = request.SessionToken;
        }

        protected virtual bool IsValidSessionToken(RequestBase request)
        {
            throw new NotImplementedException();
        }

        private void LogRequest(RequestBase request)
        {
            if (_logger != null)
            {
                string strRequest = request.ToJson();
                _logger.Log(EErrorType.Info, string.Format("Request:\t{0}", strRequest));
            }
        }
        #endregion


    }
}
