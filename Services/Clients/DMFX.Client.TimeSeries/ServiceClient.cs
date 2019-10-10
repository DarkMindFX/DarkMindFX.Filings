using DMFX.Client.TimeSeries.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;

namespace DMFX.Client.TimeSeries
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

        public GetTickerListResponse PostGetTickerList(GetTickerList request)
        {
            var response = _client.Post<GetTickerListResponse>(request);

            return response;
        }

        public GetTimeSeriesResponse PostGetTimeSeries(GetTimeSeries request)
        {
            var response = _client.Post<GetTimeSeriesResponse>(request);

            return response;
        }

        public GetTimeSeriesInfoResponse PostGetTimeSeriesInfo(GetTimeSeriesInfo request)
        {
            var response = _client.Post<GetTimeSeriesInfoResponse>(request);

            return response;
        }
        
    }
}
