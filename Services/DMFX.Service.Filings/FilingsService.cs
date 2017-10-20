﻿using DMFX.Interfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Filings
{
    public class FilingsService : ServiceBase
    {
        private IDictionary _dictionary = null;
        private Interfaces.DAL.IDal _dal = null;
        CompositionContainer _compContainer = null;


        public FilingsService()
        {
            _dictionary = Global.Container.GetExport<IDictionary>("DB").Value;
            _compContainer = Global.Container;
            InitDAL();
        }

    

        public object Any(GetRegulators request)
        {
            DateTime dtStart = DateTime.UtcNow;
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
                    List<Interfaces.CompanyInfo> companies = _dictionary.GetCompaniesByRegulator(request.RegulatorCode);
                    var rnd = new Random(DateTime.UtcNow.Millisecond);
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

            DateTime dtEnd = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetCompanyFilingsInfo\tTime:{0}", dtEnd - dtStart));

            return response;
        }

        public object Any(GetFilingData request)
        {
            DateTime dtStart = DateTime.UtcNow;
            _logger.Log(EErrorType.Info, " ****** Call start: GetFilingData");
            
            GetFilingDataResponse response = new GetFilingDataResponse();

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

                    response.CompanyCode = request.CompanyCode;
                    response.RegulatorCode = request.RegulatorCode;
                    response.FilingName = filingDataResult.FilingInfo.Name;
                    response.PeriodStart = filingDataResult.FilingInfo.PeriodStart;
                    response.PeriodEnd = filingDataResult.FilingInfo.PeriodEnd;
                    response.Submitted = filingDataResult.FilingInfo.Submitted;
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

            DateTime dtEnd = DateTime.UtcNow;

            _logger.Log(EErrorType.Info, string.Format(" ****** Call end: GetFilingData\tTime:{0}", dtEnd - dtStart));

            return response;
        }


        #region Support methods
        
        private void InitDAL()
        {
            _logger.Log(EErrorType.Info, string.Format("InitDAL: Connecting to '{0}'", ConfigurationManager.AppSettings["ConnectionStringFilings"]));

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

        private EErrorCodes ValidateSession(string sessionToken)
        {
            EErrorCodes result = EErrorCodes.InvalidSession;

            if (sessionToken == ConfigurationManager.AppSettings["ServiceSessionToken"])
            {
                result = EErrorCodes.Success;
            }
            else
            {
                Interfaces.DAL.SessionInfo sinfo = new Interfaces.DAL.SessionInfo();

                sinfo.SessionId = !string.IsNullOrEmpty(sessionToken) ? sessionToken : string.Empty;

                sinfo = _dal.GetSessionInfo(sinfo, true);
                if (sinfo != null)
                {
                    result = EErrorCodes.Success;
                }
            }

            return result;
        }
        #endregion
    }
}