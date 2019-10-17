using DMFX.Client.Filings.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Client.Filings
{
    public class ServiceClient : ServiceClientBase
    {
        string serviceUrl = null;

        public ServiceClient()
        {
            serviceUrl = Resources.ServiceUrl;
            ServiceClientBaseInitParams initParams = new ServiceClientBaseInitParams();
            initParams.ServiceUrl = serviceUrl;

            Init(initParams);
        }

        public GetCommonSizeBalanceSheetResponse PostGetCommonSizeBalanceSheet(GetCommonSizeBalanceSheet request)
        {
            var response = Post<GetCommonSizeBalanceSheet, GetCommonSizeBalanceSheetResponse>("GetCommonSizeBalanceSheet", request);

            return response;
        }

        public GetCompaniesResponse PostGetCompanies(GetCompanies request)
        {
            var response = Post<GetCompanies, GetCompaniesResponse>("GetCompanies", request);

            return response;
        }

        public GetCompanyFilingsInfoResponse PostGetCompanyFilingsInfo(GetCompanyFilingsInfo request)
        {
            var response = Post<GetCompanyFilingsInfo, GetCompanyFilingsInfoResponse>("GetCompanyFilingsInfo", request);

            return response;
        }

        public GetFilingDataResponse PostGetFilingData(GetFilingData request)
        {
            var response = Post<GetFilingData, GetFilingDataResponse>("GetFilingData", request);

            return response;
        }

        public GetFilingRatiosResponse PostGetFilingRatios(GetFilingRatios request)
        {
            var response = Post<GetFilingRatios, GetFilingRatiosResponse>("GetFilingRatios", request);

            return response;
        }

        public GetRegulatorsResponse PostGetRegulators(GetRegulators request)
        {
            var response = Post<GetRegulators, GetRegulatorsResponse> ("GetRegulators", request);

            return response;
        }
    }
}
