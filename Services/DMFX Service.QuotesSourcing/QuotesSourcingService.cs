using DMFX.Interfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using DMFX.Service.DTO.TimeSeriesSourcing;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;


namespace DMFX.Service.QuotesSourcing
{
    public class QuotesSourcingService : ServiceBase
    {
    
        public QuotesSourcingService()
        {
        }

        public object Any(ForceRunImportTimeSeries request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ForceRunImport");

            ForceRunImportResponse response = new ForceRunImportResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {

                    if (Global.Importer.CurrentState == TimeSeriesImporter.EImportState.Idle)
                    {
                        string sCompanies = string.Empty;

                        // preparing parameters for import
                        TimeSeriesImporterParams impParams = new TimeSeriesImporterParams();
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
                        impParams.Tickers = new System.Collections.Generic.HashSet<string>(request.SymbolCodes);


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

        public object Any(ForceStopImportTimeSeries request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ForceStopImport");

            ForceStopImportResponse response = new ForceStopImportResponse();

            try
            {

                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {

                    if (Global.Importer.CurrentState != TimeSeriesImporter.EImportState.Idle)
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

        public object Any(GetImporterTimeSeriesState request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetImporterTimeSeriesState");

            GetImporterTimeSeriesStateResponse response = new GetImporterTimeSeriesStateResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    _logger.Log(EErrorType.Info, string.Format("Current Importer state: {0}", Global.Importer.CurrentState.ToString()));
                    response.State = Global.Importer.CurrentState.ToString();
                    response.LastImportRun = Global.Importer.ImportStart;
                    response.LastImportEnd = Global.Importer.ImportEnd != DateTime.MinValue ? (DateTime?)Global.Importer.ImportEnd : null;
                    foreach (var cmp in Global.Importer.TickersProcessed)
                    {
                        DTO.TickerInfo info = new DTO.TickerInfo() { Code = cmp };
                        response.TimeSeriesProcessed.Add(info);
                    }
                    response.ProcessedCount = response.TimeSeriesProcessed.Count;
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetImporterTimeSeriesState");

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