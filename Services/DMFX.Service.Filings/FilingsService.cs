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

        public FilingsService()
        {
            _dictionary = Global.Container.GetExport<IDictionary>("DB").Value;
            _compContainer = Global.Container;
            InitDAL();
        }

        public object Any(GetRegulators request)
        {
            GetRegulatorsResponse response = new GetRegulatorsResponse();

            TransferHeader(request, response);

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
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
            }

            return response;
        }

        public object Any(GetCompanies request)
        {
            GetCompaniesResponse response = new GetCompaniesResponse();
            TransferHeader(request, response);

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
                        // TODO: need to fill from DAL - last available filing
                        LastFiling = new DTO.CompanyFilingInfo(),
                        // TODO: this value should be updated from DAL - for now just random 30 to 90 days
                        LastUpdate = DateTime.Now - TimeSpan.FromDays(rnd.Next(30, 90))
                    });
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
            }

            return response;
        }

        public object Any(GetCompanyFilingsInfo request)
        {
            GetCompanyFilingsInfoResponse response = new GetCompanyFilingsInfoResponse();

            TransferHeader(request, response);

            try
            {
                Interfaces.DAL.GetCompanyFilingsInfoParams infoParams = new Interfaces.DAL.GetCompanyFilingsInfoParams();
                infoParams.CompanyCode = request.CompanyCode;
                infoParams.PeriodEnd = request.PeriodEnd;
                infoParams.PeriodStart = request.PeriodStart;
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
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
            }

            return response;
        }

        public object AnyGetFilingData(GetFilingData request)
        {
            GetFilingDataResponse response = new GetFilingDataResponse();

            try
            {
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = string.Format("Unpexcted error: {0}", ex.Message) });
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
            Lazy<Interfaces.DAL.IDal> dal = _compContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionString", ConfigurationManager.AppSettings["ConnectionString"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }
        #endregion
    }
}