using DMFX.Interfaces;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMFX.Service.Filings
{
    public class FilingsService : ServiceStack.Service
    {
        private IDictionary _dictionary = null;
        private IDal _dal = null;

        public FilingsService()
        {
            _dictionary = Global.Container.GetExport<IDictionary>().Value;
            _dal = Global.Container.GetExport<IDal>().Value;
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
                        // TODO: need to fill from DAL
                        LastFiling = new CompanyFilingInfo(),
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
        #endregion
    }
}