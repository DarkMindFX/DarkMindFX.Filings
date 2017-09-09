using DMFX.Interfaces;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Sourcing
{
    public class SourcingService : ServiceStack.Service
    {
        private ILogger _logger = null;

        public SourcingService()
        {
            _logger = Global.Container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]).Value;
        }

        public object Any(Echo request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: Echo");

            EchoResponse response = new EchoResponse();

            try
            {
                TransferHeader(request, response);

                response.Message = request.Message;
                response.Success = true;
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

        public object Any(GetImportLastRun request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetImportLastRun");
            GetImportLastRunResponse response = new GetImportLastRunResponse();

            try
            {

                TransferHeader(request, response);

                response.LastRun = Global.Importer.LastRun;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: GetImportLastRun");

            return response;
        }

        public object Any(GetImportCurrentState request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetImportCurrentState");

            GetImportCurrentStateResponse response = new GetImportCurrentStateResponse();

            try
            {

                TransferHeader(request, response);

                response.State = Global.Importer.CurrentState.ToString();
                response.Success = true;
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetImportCurrentState");

            return response;
        }

        public object Any(ForceRunImport request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ForceRunImport");

            ForceRunImportResponse response = new ForceRunImportResponse();

            try
            {
            
            TransferHeader(request, response);

            if (Global.Importer.CurrentState == Importer.EImportState.Idle)
            {
                response.Success = Global.Importer.StartImport();
            }
            else
            {
                response.Errors.Add(new Interfaces.Error() { Code = Interfaces.EErrorCodes.ImporterError, Type = Interfaces.EErrorType.Error, Message = string.Format("Importing is running, current state - {0}", Global.Importer.CurrentState) });
                response.Success = false;
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

            _logger.Log(EErrorType.Info, " ****** Call end: ForceRunImport");

            return response;
        }

        #region Support methods
        protected void TransferHeader(RequestBase request, ResponseBase response)
        {
            response.RequestID = request.RequestID;
            response.SessionToken = request.SessionToken;
        }
        #endregion
    }
}