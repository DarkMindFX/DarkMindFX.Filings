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

        public object Any(GetImporterState request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetImporterState");

            GetImporterStateResponse response = new GetImporterStateResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    _logger.Log(EErrorType.Info, string.Format("Current Importer state: {0}", Global.Importer.CurrentState.ToString()));
                    response.State = Global.Importer.CurrentState.ToString();
                    response.LastImportRun = Global.Importer.ImportStart;
                    response.LastImportEnd = Global.Importer.ImportEnd != DateTime.MinValue ? (DateTime?)Global.Importer.ImportEnd : null;
                    foreach (var cmp in Global.Importer.CompaniesProcessed)
                    {
                        DTO.CompanyInfo cmpInfo = new DTO.CompanyInfo() { Code = cmp };
                        response.CompaniesProcessed.Add(cmpInfo);
                    }
                    response.ProcessedCount = response.CompaniesProcessed.Count;
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetImporterState");

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
                        impParams.DateEnd = request.DateEnd != null ? (DateTime)request.DateEnd : DateTime.UtcNow;
                        impParams.RegulatorCode = request.RegulatorCode;
                        if (request.CompanyCodes != null)
                        {
                            foreach (var c in request.CompanyCodes)
                            {
                                if (!impParams.CompanyCodes.Contains(c))
                                {
                                    impParams.CompanyCodes.Add(c);
                                }
                            }
                        }

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

        public object Any(ForceStopImport request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ForceStopImport");

            ForceStopImportResponse response = new ForceStopImportResponse();

            try
            {

                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {

                    if (Global.Importer.CurrentState != Importer.EImportState.Idle)
                    {
                        // starting import process
                        response.Success = Global.Importer.StopImport();
                        _logger.Log(EErrorType.Info, "\tImporter stopped");
                    }
                    else
                    {
                        response.Errors.Add(new Interfaces.Error() { Code = Interfaces.EErrorCodes.ImporterError, Type = Interfaces.EErrorType.Error, Message = string.Format("Importing is not running, current state - {0}", Global.Importer.CurrentState) });
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