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

                if (IsValidSessionToken(request))
                {
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

        public object Any(GetImportLastRun request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetImportLastRun");
            GetImportLastRunResponse response = new GetImportLastRunResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    response.LastRun = Global.Importer.LastRun;
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

                if (IsValidSessionToken(request))
                {
                    response.State = Global.Importer.CurrentState.ToString();
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

                if (IsValidSessionToken(request))
                {

                    if (Global.Importer.CurrentState == Importer.EImportState.Idle)
                    {
                        // preparing parameters for import
                        ImporterParams impParams = new ImporterParams();
                        impParams.DateStart = request.DateStart != null ? (DateTime)request.DateStart : DateTime.MinValue;
                        impParams.DateEnd = request.DateEnd != null ? (DateTime)request.DateEnd : DateTime.Now;
                        impParams.RegulatorCode = request.RegulatorCode;
                        impParams.CompanyCode = request.CompanyCode;

                        // starting import process
                        response.Success = Global.Importer.StartImport(impParams);
                    }
                    else
                    {
                        response.Errors.Add(new Interfaces.Error() { Code = Interfaces.EErrorCodes.ImporterError, Type = Interfaces.EErrorType.Error, Message = string.Format("Importing is running, current state - {0}", Global.Importer.CurrentState) });
                        response.Success = false;
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

        private bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }
        #endregion
    }
}