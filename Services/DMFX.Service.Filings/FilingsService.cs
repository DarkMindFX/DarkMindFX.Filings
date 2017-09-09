using DMFX.Interfaces;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Filings
{
    public class FilingsService : ServiceStack.Service
    {
        private IDictionary _dictionary = null;
        private Interfaces.DAL.IDal _dal = null;
        CompositionContainer _compContainer = null;
        private ILogger _logger = null;

        public FilingsService()
        {
            _dictionary = Global.Container.GetExport<IDictionary>("DB").Value;
            _compContainer = Global.Container;
            _logger = Global.Container.GetExport<ILogger>(ConfigurationManager.AppSettings["LoggerType"]).Value;
            InitDAL();
        }

        public object Any(GetRegulators request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetRegulators");
            GetRegulatorsResponse response = new GetRegulatorsResponse();

            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);
            if (valSession == EErrorCodes.Success)
            {
                try
                {
                    List<Interfaces.RegulatorInfo> regulators = _dictionary.GetRegulators();

                    foreach (var reg in regulators)
                    {
                        DTO.RegulatorInfo regulatorInfo = new DTO.RegulatorInfo()
                        {
                            Name = reg.Name,
                            Code = reg.Code,
                            CountryCode = reg.CountryCode
                        };

                        response.Regulators.Add(regulatorInfo);
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetRegulators");

            return response;
        }

        public object Any(GetCompanies request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetCompanies");
            GetCompaniesResponse response = new GetCompaniesResponse();
            TransferHeader(request, response);

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    List<Interfaces.CompanyInfo> companies = _dictionary.GetCompaniesByRegulator(request.RegulatorCode);
                    var rnd = new Random(DateTime.Now.Millisecond);
                    foreach (var c in companies)
                    {
                        response.Companies.Add(new DTO.CompanyInfo()
                        {
                            Name = c.Name,
                            Code = c.Code,
                            LastUpdate = c.LastUpdated != DateTime.MinValue ? (DateTime?)c.LastUpdated : null
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetCompanies");

            return response;
        }

        public object Any(GetCompanyFilingsInfo request)
        {
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

                        response.Filings.Add(cfi);
                    }
                    response.CompanyCode = request.CompanyCode;
                    response.RegulatorCode = request.RegulatorCode;
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetCompanyFilingsInfo");

            return response;
        }

        public object Any(GetFilingData request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: GetFilingData");
            GetFilingDataResponse response = new GetFilingDataResponse();

            EErrorCodes valSession = ValidateSession(request.SessionToken);

            if (valSession == EErrorCodes.Success)
            {

                try
                {
                    Interfaces.DAL.GetCompanyFilingParams filingDataParams = new Interfaces.DAL.GetCompanyFilingParams()
                    {
                        CompanyCode = request.CompanyCode,
                        Name = request.FilingName,
                        RegulatorCode = request.RegulatorCode
                    };

                    Interfaces.DAL.GetCompanyFilingResult filingDataResult = _dal.GetCompanyFilingData(filingDataParams);

                    response.CompanyCode = request.CompanyCode;
                    response.RegulatorCode = request.RegulatorCode;
                    response.FilingName = filingDataResult.FilingInfo.Name;
                    response.PeriodStart = filingDataResult.FilingInfo.PeriodStart.ToString();
                    response.PeriodEnd = filingDataResult.FilingInfo.PeriodEnd.ToString();
                    response.Submitted = filingDataResult.FilingInfo.Submitted.ToString();
                    response.Type = filingDataResult.FilingInfo.Type;

                    foreach (var fd in filingDataResult.Data)
                    {
                        response.FilingData.Add(new DTO.FilingRecord()
                        {
                            Code = fd.Code,
                            Value = fd.Value,
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

            _logger.Log(EErrorType.Info, " ****** Call end: GetFilingData");

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

        private EErrorCodes ValidateSession(string sessionToken)
        {
            EErrorCodes result = EErrorCodes.InvalidSession;

            Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();

            sinfo.SessionId = !string.IsNullOrEmpty(sessionToken) ? sessionToken : string.Empty;

            sinfo = _dal.GetSessionInfo(sinfo, true);
            if (sinfo != null)
            {
                result = EErrorCodes.Success;
            }

            return result;
        }
        #endregion
    }
}