using DMFX.Interfaces;
using DMFX.Service.Mail.MessageGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Xml;

namespace DMFX.Service.Mail
{
    public class MailParams
    {
        public MailParams()
        {
            Parameters = new Dictionary<string, object>();
        }
        public string Type
        {
            get;
            set;
        }

        public string ToAddress
        {
            get;
            set;
        }

        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }
    public class Mailer
    {
        private MailSender _mailSender = new MailSender();
        private ILogger _logger = null;
        private CompositionContainer _compContainer = null;

        public Mailer(ILogger logger, MailSenderSettings senderSettings, CompositionContainer compContainer)
        {
            _logger = logger;
            _compContainer = compContainer;
            _mailSender.Init(senderSettings);
        }

        public ResultBase Send(List<MailParams> mails)
        {
            ResultBase result = new ResultBase();
            List<EmailDetails> emails = new List<EmailDetails>();
            foreach (var mp in mails)
            {
                try
                {
                    EmailDetails emailDetails = PrepareEmailDetails(mp.Type, mp.ToAddress, mp.Parameters);
                    emails.Add(emailDetails);
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new Error() { Code = EErrorCodes.MailSendFailed, Message = ex.Message, Type = EErrorType.Warning });
                    _logger.Log(EErrorType.Error, string.Format("Failed to prepare email details:\r\n\tTo:  {0}\r\n\tType: {1}\r\nError:  {2}", mp.ToAddress, mp.Type, ex.Message));
                }
            }

            try
            {
                if (emails.Count > 0)
                {
                    _mailSender.Send(emails);
                    result.Success = true;
                }
                else
                {
                    throw new ArgumentNullException("No email details were prepared");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(new Error() { Code = EErrorCodes.MailSendFailed, Message = ex.Message, Type = EErrorType.Error });
                _logger.Log(ex);
            }

            return result;
        }

        #region Support methods
        private EmailDetails PrepareEmailDetails(string type, string toAddress, Dictionary<string, object> parameters)
        {
            EmailDetails emailDetails = new EmailDetails();
            emailDetails.AddressTo = toAddress;

            var msgGen = _compContainer.GetExport<IMessageGenerator>(type);

            if (msgGen != null)
            {
                // getting xml
                string xmlConfig = Resources.ResourceManager.GetObject(type) as string;
                if (xmlConfig != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlConfig);

                    // extracting subject
                    XmlNode nodeSubject = doc.SelectSingleNode("template/subject");
                    if (nodeSubject != null)
                    {
                        emailDetails.Subject = nodeSubject.InnerText.Trim();
                    }

                    // extracting message template
                    XmlNode nodeBody = doc.SelectSingleNode("template/body");
                    if (nodeBody != null)
                    {
                        IMessageGeneratorParams msgGenParams = msgGen.Value.CreateParams();
                        msgGenParams.Parameters = parameters;
                        msgGenParams.Template = nodeBody.InnerXml;

                        emailDetails.Body = msgGen.Value.CreateMessage(msgGenParams);
                    }              
                }
                else
                {
                    throw new ArgumentException(string.Format("XmlConfig not found for type {0}", type));
                }

                

            }
            else
            {
                throw new ArgumentException(string.Format("Message Generator of type {0} not found", type));
            }

            return emailDetails;
        }
        #endregion
    }
}