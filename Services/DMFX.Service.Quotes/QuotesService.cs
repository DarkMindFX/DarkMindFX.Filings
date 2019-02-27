using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Quotes
{
    public class QuotesService : ServiceBase
    {
        CompositionContainer _compContainer = null;

        public QuotesService()
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