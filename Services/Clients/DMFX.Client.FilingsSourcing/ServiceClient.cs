using DMFX.Client.FilingsSourcing.Properties;
using DMFX.Client.Common;
using DMFX.Service.DTO;


namespace DMFX.Client.FilingsSourcing
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

        public ForceRunImportResponse PostForceRunImport(ForceRunImport request)
        {
            ForceRunImportResponse response = _client.Post<ForceRunImportResponse>(request);

            return response;
        }

        public ForceStopImportResponse PostForceStopImport(ForceStopImport request)
        {
            ForceStopImportResponse response = _client.Post<ForceStopImportResponse>(request);

            return response;
        }

        public GetImporterStateResponse PostGetImporterState(GetImporterState request)
        {
            GetImporterStateResponse response = _client.Post<GetImporterStateResponse>(request);

            return response;
        }


    }
}
