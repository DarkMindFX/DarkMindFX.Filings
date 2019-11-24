using DMFX.Interfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Sourcing
{
    public class SourcingService : ServiceBase
    {

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
                    response.Payload.State = Global.Importer.CurrentState.ToString();
                    response.Payload.LastImportRun = Global.Importer.ImportStart;
                    response.Payload.LastImportEnd = Global.Importer.ImportEnd != DateTime.MinValue ? (DateTime?)Global.Importer.ImportEnd : null;
                    foreach (var cmp in Global.Importer.CompaniesProcessed)
                    {
                        DTO.CompanyInfo cmpInfo = new DTO.CompanyInfo() { Code = cmp };
                        response.Payload.CompaniesProcessed.Add(cmpInfo);
                    }
                    response.Payload.ProcessedCount = response.Payload.CompaniesProcessed.Count;
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
                        string sCompanies = string.Empty;
                        string sTypes = string.Empty;
                        
                        // preparing parameters for import
                        ImporterParams impParams = new ImporterParams();
                        if (request.DaysBack != null)
                        {
                            impParams.DateEnd = DateTime.UtcNow;
                            impParams.DateStart = impParams.DateEnd - TimeSpan.FromDays((double)request.DaysBack);
                        }
                        else
                        {
                            impParams.DateStart = request.DateStart != null ? (DateTime)request.DateStart : DateTime.Parse(ConfigurationManager.AppSettings["UpdateFromDate"]);
                            impParams.DateEnd = request.DateEnd != null ? (DateTime)request.DateEnd : DateTime.UtcNow;
                        }
                        impParams.RegulatorCode = request.RegulatorCode;
                        if (request.CompanyCodes != null)
                        {
                            foreach (var c in request.CompanyCodes)
                            {
                                if (!impParams.CompanyCodes.Contains(c))
                                {
                                    sCompanies += " " + c;
                                    impParams.CompanyCodes.Add(c);
                                }
                            }
                        }
                        if (request.Types != null)
                        {
                            foreach (var t in request.Types)
                            {
                                if (!impParams.Types.Contains(t))
                                {
                                    sTypes += " " + t;
                                    impParams.Types.Add(t);
                                }
                            }
                        }

                        _logger.Log(EErrorType.Info, 
                            string.Format(" ****** Starting import: \r\n\tFrom {0} to {1},\r\n\tRegulator: {2},\r\n\tCompanies: {3}\r\n\tTypes: {4}", 
                                impParams.DateStart, impParams.DateEnd, 
                                !string.IsNullOrEmpty(impParams.RegulatorCode) ? impParams.RegulatorCode : "All" , 
                                !string.IsNullOrEmpty(sCompanies) ? sCompanies : "All",
                                !string.IsNullOrEmpty(sTypes) ? sTypes : "All"));

                        // starting import process
                        response.Success = Global.Importer.StartImport(impParams);
                    }
                    else
                    {
                        response.Errors.Add(new Interfaces.Error() { Code = Interfaces.EErrorCodes.ImporterBusy, Type = Interfaces.EErrorType.Error, Message = string.Format("Importing is running, current state - {0}", Global.Importer.CurrentState) });
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
        
        protected override bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }
        #endregion
    }
}