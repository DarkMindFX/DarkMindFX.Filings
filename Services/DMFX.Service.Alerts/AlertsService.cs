using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Web;

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