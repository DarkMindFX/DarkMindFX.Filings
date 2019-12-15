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
            ForceRunImportResponse response = Post<ForceRunImport, ForceRunImportResponse>("ForceRunImport", request);

            return response;
        }

        public ForceStopImportResponse PostForceStopImport(ForceStopImport request)
        {
            ForceStopImportResponse response = Post<ForceStopImport, ForceStopImportResponse>("ForceStopImport", request);

            return response;
        }

        public GetImporterStateResponse PostGetImporterState(GetImporterState request)
        {
            GetImporterStateResponse response = Post<GetImporterState, GetImporterStateResponse>("GetImporterState", request);

            return response;
        }


    }
}
