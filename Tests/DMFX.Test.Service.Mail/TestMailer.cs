using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DMFX.Test.Service.Common;
using DMFX.Service.Mail;
using System.Configuration;
using DMFX.Logging;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using DMFX.Service.Mail.MessageGenerators;
using DMFX.Interfaces;

namespace DMFX.Test.Service.Mail
{
    [TestFixture]
    class TestMailer : TestBase
    {
        private CompositionContainer _container = null;

        [SetUp]
        public void SetUp()
        {
            TypeCatalog typeCatalog = new TypeCatalog(
                typeof(MsgGenAccountCreatedConfirmation),
                typeof(MsgGenNewReportNotification),
                typeof(MsgGenPasswordChangedConfirmation),
                typeof(MsgGenPasswordResetNotification)
                );

            _container = new CompositionContainer(typeCatalog);
            _container.ComposeParts(this);
        }

        [Test]
        public void SendMail_AccountCreatedConfirmation_Success()
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

                NullLogger logger = new NullLogger();

                Mailer mailer = new Mailer(logger, senderSettings, _container);

                MailParams mailParams = new MailParams();
                mailParams.ToAddress =  ConfigurationManager.AppSettings["AddressTo"];
                mailParams.Type =       ConfigurationManager.AppSettings["TypeAccountCreatedConfirmation"];

                mailParams.Parameters.Add("UserName", ConfigurationManager.AppSettings["TestUser01Name"]);
                mailParams.Parameters.Add("Login", ConfigurationManager.AppSettings["AddressTo"]);
                mailParams.Parameters.Add("AccountKey", "7A522D74-1A0B-403C-B504-7E9634C08ECF");

                List<MailParams> emailsParams = new List<MailParams>();
                emailsParams.Add(mailParams);

                ResultBase result = mailer.Send(emailsParams);

                Assert.IsTrue(result.Success, "Mail was not sent");
                Assert.IsTrue(!result.HasErrors, "Unexpected errors were returned");
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Sending email failed: {0}", ex.Message));
            }

        }

        [Test]
        public void SendMail_NewReportNotification_Success()
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

                NullLogger logger = new NullLogger();

                Mailer mailer = new Mailer(logger, senderSettings, _container);

                MailParams mailParams = new MailParams();
                mailParams.ToAddress = ConfigurationManager.AppSettings["AddressTo"];
                mailParams.Type = ConfigurationManager.AppSettings["TypeNewReportNotification"];

                mailParams.Parameters.Add("UserName", ConfigurationManager.AppSettings["TestUser01Name"]);
                mailParams.Parameters.Add("RegulatorCode", ConfigurationManager.AppSettings["RegulatorCode"]);
                mailParams.Parameters.Add("CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                mailParams.Parameters.Add("ReportType", ConfigurationManager.AppSettings["ReportType"]);
                mailParams.Parameters.Add("ReportName", ConfigurationManager.AppSettings["ReportName"]);
                mailParams.Parameters.Add("ReportSubmitted", ConfigurationManager.AppSettings["ReportSubmitted"]);

                List<MailParams> emailsParams = new List<MailParams>();
                emailsParams.Add(mailParams);

                ResultBase result = mailer.Send(emailsParams);

                Assert.IsTrue(result.Success, "Mail was not sent");
                Assert.IsTrue(!result.HasErrors, "Unexpected errors were returned");
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Sending email failed: {0}", ex.Message));
            }

        }

        [Test]
        public void SendMail_PasswordChangedConfirmation_Success()
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

                NullLogger logger = new NullLogger();

                Mailer mailer = new Mailer(logger, senderSettings, _container);

                MailParams mailParams = new MailParams();
                mailParams.ToAddress = ConfigurationManager.AppSettings["AddressTo"];
                mailParams.Type = ConfigurationManager.AppSettings["TypePasswordChangedConfirmation"];

                mailParams.Parameters.Add("UserName", ConfigurationManager.AppSettings["TestUser01Name"]);

                List<MailParams> emailsParams = new List<MailParams>();
                emailsParams.Add(mailParams);

                ResultBase result = mailer.Send(emailsParams);

                Assert.IsTrue(result.Success, "Mail was not sent");
                Assert.IsTrue(!result.HasErrors, "Unexpected errors were returned");
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Sending email failed: {0}", ex.Message));
            }

        }

        [Test]
        public void SendMail_PasswordResetNotification_Success()
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

                NullLogger logger = new NullLogger();

                Mailer mailer = new Mailer(logger, senderSettings, _container);

                MailParams mailParams = new MailParams();
                mailParams.ToAddress = ConfigurationManager.AppSettings["AddressTo"];
                mailParams.Type = ConfigurationManager.AppSettings["TypePasswordResetNotification"];

                mailParams.Parameters.Add("UserName", ConfigurationManager.AppSettings["TestUser01Name"]);
                mailParams.Parameters.Add("Password", "New Password Reset");

                List<MailParams> emailsParams = new List<MailParams>();
                emailsParams.Add(mailParams);

                ResultBase result = mailer.Send(emailsParams);

                Assert.IsTrue(result.Success, "Mail was not sent");
                Assert.IsTrue(!result.HasErrors, "Unexpected errors were returned");
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Sending email failed: {0}", ex.Message));
            }

        }
    }
}
