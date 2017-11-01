using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.TechUtils
{
    public class TechUtilsService : ServiceBase
    {

        public TechUtilsService()
        {
        }        

        public object Any(Sanitize request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: Sanitize");

            EchoResponse response = new EchoResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    if (ConfigurationManager.AppSettings["CanSanitize"] != null && bool.Parse(ConfigurationManager.AppSettings["CanSanitize"]))
                    {
                        TechUtils tu = new TechUtils();
                        tu.Sanitize();
                        _logger.Log(EErrorType.Info, string.Format("Sanitization started"));
                    }
                    else
                    {
                        _logger.Log(EErrorType.Warning, string.Format("Sanitization not allowed - start cancelled"));
                    }

                    
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unpexcted error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: Sanitize");

            return response;
        }

        public object Any(ClearLayer request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: ClearLayer");

            EchoResponse response = new EchoResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    string checkFlag = "CanClear" + request.LayerCode;
                    if (ConfigurationManager.AppSettings[checkFlag] != null && bool.Parse(ConfigurationManager.AppSettings[checkFlag]))
                    {

                        TechUtils tu = new TechUtils();
                        tu.ClearLayer(request.LayerCode);
                    }

                    _logger.Log(EErrorType.Info, string.Format("ClearLayer started"));
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error() { Code = EErrorCodes.InvalidSession, Type = EErrorType.Error, Message = "Invalid session token" });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                response.Success = false;
                response.Errors.Add(new Error()
                {
                    Code = EErrorCodes.GeneralError,
                    Type = EErrorType.Error,
                    Message = string.Format("Unpexcted error: {0}", ex.Message)
                });
            }

            _logger.Log(EErrorType.Info, " ****** Call end: ClearLayer");

            return response;
        }

        #region Support methods

        protected override bool IsValidSessionToken(RequestBase request)
        {
            return request.SessionToken != null && request.SessionToken.Equals(ConfigurationManager.AppSettings["ServiceSessionToken"]);
        }
        #endregion
    }
}