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
            var response = Post<GetTickerList, GetTickerListResponse>("GetTickerList", request);

            return response;
        }

        public GetTimeSeriesResponse PostGetTimeSeries(GetTimeSeries request)
        {
            var response = Post<GetTimeSeries, GetTimeSeriesResponse>("GetTimeSeries", request);

            return response;
        }

        public GetTimeSeriesInfoResponse PostGetTimeSeriesInfo(GetTimeSeriesInfo request)
        {
            var response = Post<GetTimeSeriesInfo, GetTimeSeriesInfoResponse>("GetTimeSeriesInfo", request);

            return response;
        }
        
    }
}
