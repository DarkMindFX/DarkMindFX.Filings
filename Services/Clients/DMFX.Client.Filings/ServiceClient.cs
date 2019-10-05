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
            var response = _client.Post<GetCommonSizeBalanceSheetResponse>(request);

            return response;
        }

        public GetCompaniesResponse PostGetCompanies(GetCompanies request)
        {
            var response = _client.Post<GetCompaniesResponse>(request);

            return response;
        }

        public GetCompanyFilingsInfoResponse PostGetCompanyFilingsInfo(GetCompanyFilingsInfo request)
        {
            var response = _client.Post<GetCompanyFilingsInfoResponse>(request);

            return response;
        }

        public GetFilingDataResponse PostGetFilingData(GetFilingData request)
        {
            var response = _client.Post<GetFilingDataResponse>(request);

            return response;
        }

        public GetFilingRatiosResponse PostGetFilingRatios(GetFilingRatios request)
        {
            var response = _client.Post<GetFilingRatiosResponse>(request);

            return response;
        }

        public GetRegulatorsResponse PostGetRegulators(GetRegulators request)
        {
            var response = _client.Post<GetRegulatorsResponse> (request);

            return response;
        }
    }
}
