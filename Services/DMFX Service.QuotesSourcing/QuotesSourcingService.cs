using DMFX.Service.Common;
using DMFX.Service.DTO;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;


namespace DMFX.Service.QuotesSourcing
{
    public class QuotesSourcingService : ServiceBase
    {

        CompositionContainer _compContainer = null;


        public QuotesSourcingService()
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