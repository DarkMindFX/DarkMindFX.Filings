using DMFX.Service.Mail;
using DMFX.Test.Service.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Test.Service.Mail
{
    [TestFixture]
    public class TestMailSender : TestBase
    {
        [Test]
        public void SendMail_Single_Success()
        {
            try
            {

                MailSenderSettings senderSettings = new MailSenderSettings();
                senderSettings.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                senderSettings.Port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
                senderSettings.SenderAddress = ConfigurationManager.AppSettings["SenderAddress"];
                senderSettings.SenderPwd = ConfigurationManager.AppSettings["SenderPwd"];
                senderSettings.SenderTitle = ConfigurationManager.AppSettings["SenderTitle"];
                senderSettings.SmtpAddress = ConfigurationManager.AppSettings["SmtpAddress"];

                MailSender mailSender = new MailSender();
                mailSender.Init(senderSettings);

                // preparing mail details
                EmailDetails emailDetails = new EmailDetails();
                emailDetails.AddressTo = ConfigurationManager.AppSettings["AddressTo"];
                emailDetails.Subject = ConfigurationManager.AppSettings["Subject"];
                emailDetails.Body = string.Format(ConfigurationManager.AppSettings["Body"], DateTime.UtcNow.ToString());

                var emails = new List<EmailDetails>();
                emails.Add(emailDetails);

                mailSender.Send(emails);

                Assert.IsTrue(true, "Mail sent - check inbox");
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Sending email failed: {0}", ex.Message));
            }
            
        }

        [Test]
        public void SendMail_Batch_Success()
        {
            try
            {

                MailSenderSettings senderSettings = new MailSenderSettings();
                senderSettings.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                senderSettings.Port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
                senderSettings.SenderAddress = ConfigurationManager.AppSettings["SenderAddress"];
                senderSettings.SenderPwd = ConfigurationManager.AppSettings["SenderPwd"];
                senderSettings.SenderTitle = ConfigurationManager.AppSettings["SenderTitle"];
                senderSettings.SmtpAddress = ConfigurationManager.AppSettings["SmtpAddress"];

                MailSender mailSender = new MailSender();
                mailSender.Init(senderSettings);

                var emails = new List<EmailDetails>();
                // preparing mail details
                for (int i = 0; i < 3; ++i)
                {
                    EmailDetails emailDetails = new EmailDetails();
                    emailDetails.AddressTo = ConfigurationManager.AppSettings["AddressTo"];
                    emailDetails.Subject = ConfigurationManager.AppSettings["Subject"];
                    emailDetails.Body = string.Format(ConfigurationManager.AppSettings["Body"], (DateTime.UtcNow + TimeSpan.FromHours(i)).ToString());

                    
                    emails.Add(emailDetails);
                }

                mailSender.Send(emails);

                Assert.IsTrue(true, "3 emails sent - check inbox");
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Sending email failed: {0}", ex.Message));
            }

        }
    }
}
