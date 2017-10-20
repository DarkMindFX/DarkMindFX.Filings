using DMFX.Interfaces;
using DMFX.Service.Common;
using DMFX.Service.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DMFX.Service.Mail
{
    public class MailService : ServiceBase
    {

           
        public object Any(SendMail request)
        {
            _logger.Log(EErrorType.Info, " ****** Call start: SendMail");
            _logger.Log(EErrorType.Info, string.Format("Sending {0} email messages", request.Details.Count));

            SendMailResponse response = new SendMailResponse();

            try
            {
                TransferHeader(request, response);

                if (IsValidSessionToken(request))
                {
                    if (request.Details.Count > 0)
                    {
                        MailSenderSettings senderSettings = new MailSenderSettings();
                        senderSettings.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                        senderSettings.Port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
                        senderSettings.SenderAddress = ConfigurationManager.AppSettings["SenderAddress"];
                        senderSettings.SenderPwd = ConfigurationManager.AppSettings["SenderPwd"];
                        senderSettings.SenderTitle = ConfigurationManager.AppSettings["SenderTitle"];
                        senderSettings.SmtpAddress = ConfigurationManager.AppSettings["SmtpAddress"];

                        Mailer mailer = new Mailer(_logger, senderSettings, Global.Container);

                        List<MailParams> emailsParams = new List<MailParams>();

                        foreach (var d in request.Details)
                        {
                            MailParams p = new MailParams();
                            p.Type = d.MessageType;
                            p.ToAddress = d.ToAddress;
                            p.Parameters = d.Parameters;

                            emailsParams.Add(p);

                            
                        }

                        mailer.Send(emailsParams);
                    }
                    else
                    {
                        response.Errors.Add(
                            new Error() {
                                Code = EErrorCodes.EmptyColection,
                                Type = EErrorType.Warning,
                                Message = "No emails sent - details were not provided" });
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

            _logger.Log(EErrorType.Info, " ****** Call end: SendMail");

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