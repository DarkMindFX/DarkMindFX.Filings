using DMFX.Service.Common;
using DMFX.Service.DTO;
using System.Configuration;

namespace DMFX.Service.Alerts
{
    public class AlertsService : ServiceBase
    {

        public AlertsService()
        {
        }


        #region Support methods

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }
        #endregion
    }
}