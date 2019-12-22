using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.MQClient;
using DMFX.MQInterfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace DMFX.Service.Filings
{
    public class FilingsService : ServiceBase
    {
        private Interfaces.DAL.IDal _dal = null;
        CompositionContainer _compContainer = null;


        public FilingsService()
        {
            _compContainer = Global.Container;
            InitDAL();
        }

        public object Any(GetRegulators request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetRegulators");
            GetRegulatorsResponse response = new GetRegulatorsResponse();

            TransferHeader(request, response);

            _logger.Log(EErrorType.Info, string.Format("ValidateSession({0})", request.SessionToken));

            EErrorCodes valSession = ValidateSession(request.SessionToken);
            if (valSession == EErrorCodes.Success)
            {
                try
                {

                    var regulators = _dal.GetRegulators();;

                    foreach (var reg in regulators.Regulators)
                    {
                        DTO.RegulatorInfo regulatorInfo = new DTO.RegulatorInfo()
                        {
                            Name = reg.Name,
                            Code = reg.Code,
                            CountryCode = reg.CountryCode
                        };

                        response.Payload.Regulators.Add(regulatorInfo);
                    }

                    response.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }
            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = valSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetRegulators\tTime: {0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetCompanies request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetCompanies");
            GetCompaniesResponse response = new GetCompaniesResponse();
            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    GetRegulatorCompaniesParams getCmpParams = new GetRegulatorCompaniesParams();
                    getCmpParams.RegulatorCode = request.RegulatorCode;
                    var companies = _dal.GetCompaniesByRegulator(getCmpParams);
                    if (companies != null && companies.Companies != null)
                    {
                        foreach (var c in companies.Companies)
                        {
                            response.Payload.Companies.Add(new DTO.CompanyInfo()
                            {
                                Name = c.Name,
                                Code = c.Code,
                                LastUpdate = c.LastUpdated != DateTime.MinValue ? (DateTime?)c.LastUpdated : null
                            });
                        }
                    }
                    else
                    {
                        response.Errors.Add(new Error()
                        {
                            Code = EErrorCodes.EmptyCollection,
                            Type = EErrorType.Warning,
                            Message = string.Format("No companies found for regulator {0}", request.RegulatorCode)
                        });
                    }

                    response.Payload.RegulatorCode = request.RegulatorCode;
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }
            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetCompanies\t{0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetCompanyFilingsInfo request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetCompanyFilingsInfo");
            GetCompanyFilingsInfoResponse response = new GetCompanyFilingsInfoResponse();

            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);
            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    Interfaces.DAL.GetCompanyFilingsInfoParams infoParams = new Interfaces.DAL.GetCompanyFilingsInfoParams();
                    infoParams.CompanyCode = request.CompanyCode;
                    infoParams.PeriodEnd = request.PeriodEnd != null ? (DateTime)request.PeriodEnd : infoParams.PeriodEnd;
                    infoParams.PeriodStart = request.PeriodStart != null ? (DateTime)request.PeriodStart : infoParams.PeriodStart;
                    infoParams.RegulatorCode = request.RegulatorCode;
                    foreach (var t in request.Types)
                    {
                        infoParams.Types.Add(t);
                    }

                    Interfaces.DAL.GetCompanyFilingsInfoResult dalResult = _dal.GetCompanyFilingsInfo(infoParams);
                    foreach (var f in dalResult.Filings)
                    {
                        DTO.CompanyFilingInfo cfi = new DTO.CompanyFilingInfo();
                        cfi.Name = f.Name;
                        cfi.PeriodEnd = f.PeriodEnd;
                        cfi.PeriodStart = f.PeriodStart;
                        cfi.Submitted = f.Submitted;
                        cfi.Type = f.Type;

                        response.Payload.Filings.Add(cfi);
                    }
                    response.Payload.CompanyCode = request.CompanyCode;
                    response.Payload.RegulatorCode = request.RegulatorCode;
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }

            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetCompanyFilingsInfo\tTime:{0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetFilingData request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetFilingData");

            GetFilingDataResponse response = new GetFilingDataResponse();

            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    Interfaces.DAL.GetCompanyFilingParams filingDataParams = new Interfaces.DAL.GetCompanyFilingParams(request.Values.ToArray())
                    {
                        CompanyCode = request.CompanyCode,
                        Name = request.FilingName,
                        RegulatorCode = request.RegulatorCode
                    };

                    Interfaces.DAL.GetCompanyFilingResult filingDataResult = _dal.GetCompanyFilingData(filingDataParams);

                    response.Payload.CompanyCode = request.CompanyCode;
                    response.Payload.RegulatorCode = request.RegulatorCode;
                    if (filingDataResult.FilingInfo != null)
                    {

                        response.Payload.FilingName = filingDataResult.FilingInfo.Name;
                        response.Payload.PeriodStart = filingDataResult.FilingInfo.PeriodStart;
                        response.Payload.PeriodEnd = filingDataResult.FilingInfo.PeriodEnd;
                        response.Payload.Submitted = filingDataResult.FilingInfo.Submitted;
                        response.Payload.Type = filingDataResult.FilingInfo.Type;

                        foreach (var fd in filingDataResult.Data)
                        {
                            if (fd.Value != null)
                            {
                                response.Payload.FilingData.Add(new DTO.FilingNumRecord()
                                {
                                    Code = fd.Code,
                                    Value = (decimal)fd.Value,
                                    UnitName = fd.Unit,
                                    PeriodEnd = fd.PeriodEnd,
                                    PeriodStart = fd.PeriodStart,
                                    FactId = fd.FactId
                                    
                                });
                            }
                            else if (fd.Value_Str != null)
                            {
                                response.Payload.FilingStrData.Add(new DTO.FilingStrRecord()
                                {
                                    Code = fd.Code,
                                    Value = fd.Value_Str,
                                    PeriodEnd = fd.PeriodEnd,
                                    PeriodStart = fd.PeriodStart,
                                    FactId = fd.FactId
                                });
                            }
                            else if (fd.Value_Dttm != null)
                            {
                                response.Payload.FilingDttmData.Add(new DTO.FilingDttmRecord()
                                {
                                    Code = fd.Code,
                                    Value = (DateTime)fd.Value_Dttm,
                                    PeriodEnd = fd.PeriodEnd,
                                    PeriodStart = fd.PeriodStart,
                                    FactId = fd.FactId
                                });
                            }

                        }

                        if (response.Payload.FilingData.Count == 0)
                        {
                            response.Payload.FilingData = null;
                        }
                        if (response.Payload.FilingStrData.Count == 0)
                        {
                            response.Payload.FilingStrData = null;
                        }


                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error()
                        {
                            Code = EErrorCodes.SubmissionNotFound,
                            Type = EErrorType.Error,
                            Message = "Filing not found"
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }

            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetFilingData\tTime:{0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetFilingRatios request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetFilingRatios");

            GetFilingRatiosResponse response = new GetFilingRatiosResponse();

            TransferHeader(request, response);

            _logger.Log(EErrorType.Info, string.Format("ValidateSession({0})", request.SessionToken));

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    Interfaces.DAL.GetCompanyFilingRatiosParams filingRatiosParams = new Interfaces.DAL.GetCompanyFilingRatiosParams(request.RatioCodes.ToArray())
                    {
                        CompanyCode = request.CompanyCode,
                        Name = request.FilingName,
                        RegulatorCode = request.RegulatorCode,
                    };

                    Interfaces.DAL.GetCompanyFilingRatiosResult filingRatiosResult = _dal.GetCompanyFilingRatios(filingRatiosParams);

                    response.Payload.CompanyCode = request.CompanyCode;
                    response.Payload.RegulatorCode = request.RegulatorCode;

                    response.Payload.FilingName = request.FilingName;

                    foreach (var fd in filingRatiosResult.Data)
                    {
                        response.Payload.Ratios.Add(new DTO.RatioRecord()
                        {
                            Code = fd.Code,
                            Value = fd.Value != null ? (decimal)fd.Value : 0,
                            UnitName = fd.Unit,
                            PeriodEnd = fd.PeriodEnd,
                            PeriodStart = fd.PeriodStart
                        });
                    }


                    response.Success = true;

                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }

            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetFilingRatios\tTime:{0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetCommonSizeBalanceSheet request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetCommonSizeBalanceSheet");

            GetCommonSizeBalanceSheetResponse response = new GetCommonSizeBalanceSheetResponse();

            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    Interfaces.DAL.GetCommonSizeBalanceSheetParams filingCSBSParams = new Interfaces.DAL.GetCommonSizeBalanceSheetParams(request.Values.ToArray())
                    {
                        CompanyCode = request.CompanyCode,
                        Name = request.FilingName,
                        RegulatorCode = request.RegulatorCode,
                    };

                    Interfaces.DAL.GetCommonSizeBalanceSheetResult filingCSBSResult = _dal.GetCommonSizeBalanceSheet(filingCSBSParams);

                    response.Payload.CompanyCode = request.CompanyCode;
                    response.Payload.RegulatorCode = request.RegulatorCode;

                    response.Payload.FilingName = request.FilingName;

                    foreach (var fd in filingCSBSResult.Data)
                    {
                        response.Payload.BalanceSheetData.Add(new DTO.FilingNumRecord()
                        {
                            Code = fd.Code,
                            Value = fd.Value != null ? (decimal)fd.Value : 0,
                            PeriodEnd = fd.PeriodEnd,
                            PeriodStart = fd.PeriodStart
                        });
                    }
                    response.Success = true;

                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }

            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetCommonSizeBalanceSheet\tTime:{0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetCommonSizeIncomeStatement request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetCommonSizeIncomeStatement");

            GetCommonSizeIncomeStatementResponse response = new GetCommonSizeIncomeStatementResponse();

            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    Interfaces.DAL.GetCommonSizeIncomeStatementParams filingCSBSParams = new Interfaces.DAL.GetCommonSizeIncomeStatementParams(request.Values.ToArray())
                    {
                        CompanyCode = request.CompanyCode,
                        Name = request.FilingName,
                        RegulatorCode = request.RegulatorCode,
                    };

                    Interfaces.DAL.GetCommonSizeIncomeStatementResult filingCSBSResult = _dal.GetCommonSizeIncomeStatement(filingCSBSParams);

                    response.Payload.CompanyCode = request.CompanyCode;
                    response.Payload.RegulatorCode = request.RegulatorCode;

                    response.Payload.FilingName = request.FilingName;

                    foreach (var fd in filingCSBSResult.Data)
                    {
                        response.Payload.IncomeStatementData.Add(new DTO.FilingNumRecord()
                        {
                            Code = fd.Code,
                            Value = fd.Value != null ? (decimal)fd.Value : 0,
                            PeriodEnd = fd.PeriodEnd,
                            PeriodStart = fd.PeriodStart
                        });
                    }
                    response.Success = true;

                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
                }

            }
            else
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = string.Format("Invalid session") });
            }

            DateTime dtEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetCommonSizeIncomeStatement\tTime:{0}", dtEnd - dtStart));

            return response;
        }


        #region Support methods

        private void InitDAL()
        {
            //_logger.Log(EErrorType.Info, string.Format("InitDAL: Connecting to '{0}'", ConfigurationManager.AppSettings["ConnectionStringFilings"]));

            Lazy<Interfaces.DAL.IDal> dal = _compContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionStringFilings", ConfigurationManager.AppSettings["ConnectionStringFilings"]);
            dalParams.Parameters.Add("ConnectionStringAccounts", ConfigurationManager.AppSettings["ConnectionStringAccounts"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return ValidateSession(request.SessionToken) == EErrorCodes.Success;
        }

        AutoResetEvent eventRespReceived = null;
        GetSessionInfoResponse sinfoResponse = null;
        GetSessionInfo sinfo = null;

        private void NewMessagesHandlerSender(object sender, NewChannelMessagesDelegateEventArgs args)
        {
            int i = 0;
            while (i < args.Messages.Count && sinfoResponse == null)
            {
                var m = args.Messages[i];
                GetSessionInfoResponse resp = null;
                if (m.ChannelName == Global.AccountsChannel)
                {
                    switch (m.Type)
                    {
                        case "GetSessionInfoResponse":
                            resp = JsonSerializer.DeserializeFromString(m.Payload, typeof(GetSessionInfoResponse)) as GetSessionInfoResponse;
                            if (resp != null && resp.RequestID == sinfo.RequestID)
                            {
                                Global.MQClient.DeleteMessage(m.Id);
                                sinfoResponse = resp;
                                // this is our message - removing it from queue and raising event to unblock the thread                                
                                eventRespReceived.Set();
                            }
                            break;
                    }
                }
                ++i;
            }
        }

        private EErrorCodes ValidateSession(string sessionToken)
        {
            EErrorCodes result = EErrorCodes.InvalidSession;
            

            if (sessionToken == ConfigurationManager.AppSettings["ServiceSessionToken"])
            {
                result = EErrorCodes.Success;
            }
            else
            {
                eventRespReceived = new AutoResetEvent(false); // this event will be raised when response received

                sinfo = new GetSessionInfo();
                sinfo.SessionToken = sessionToken;
                sinfo.CheckActive = true;
                

                int waitTimeout = Int32.Parse(ConfigurationManager.AppSettings["MQWaitTimeout"]);
                // sending message to queue
                string payload = JsonSerializer.SerializeToString(sinfo);
                string type = "GetSessionInfo";

                Global.MQClient.NewChannelMessages += NewMessagesHandlerSender;
                Global.MQClient.Push(Global.AccountsChannel, type, payload);

                _logger.Log(EErrorType.Info, string.Format("GetSessionInfo: {0}", sinfo.RequestID));

                // receiving message
                eventRespReceived.WaitOne(waitTimeout);
                Global.MQClient.NewChannelMessages -= NewMessagesHandlerSender;

                if (sinfoResponse != null)
                {
                    if (!sinfoResponse.Success)
                    {
                        _logger.Log(EErrorType.Warning, string.Format("sinfoResponse error: {0}, {1}", sinfo.RequestID, sinfoResponse.Errors[0].Code));
                    }
                    result = sinfoResponse.Success ? EErrorCodes.Success : sinfoResponse.Errors[0].Code;
                }
                else
                {
                    _logger.Log(EErrorType.Warning, string.Format("MQCommunicationError: {0}", sinfo.RequestID));
                    result = EErrorCodes.MQCommunicationError;
                }

            }

            return result;
        }
        #endregion
    }
}