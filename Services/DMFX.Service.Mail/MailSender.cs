using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace DMFX.Service.Mail
{
    public class EmailDetails
    {
        public string AddressTo
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }
    }

    public class MailSenderSettings
    {
        public string SmtpAddress
        {
            get;
            set;
        }

        public bool EnableSsl
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string SenderAddress
        {
            get;
            set;
        }

        public string SenderPwd
        {
            get;
            set;
        }

        public string SenderTitle
        {
            get;
            set;
        }
    }

    public class MailSender
    {
        private string  _smtpAddress = string.Empty;
        private bool    _enableSsl = true;
        private int     _port = 0;
        private string _senderAddress = string.Empty;
        private string _senderPwd = string.Empty;
        private string _senderTitle = string.Empty;

        public void Init(MailSenderSettings settings)
        {
            _smtpAddress =      settings.SmtpAddress;
            _enableSsl =        settings.EnableSsl;
            _port =             settings.Port;
            _senderAddress =    settings.SenderAddress;
            _senderPwd =        settings.SenderPwd;
            _senderTitle =      settings.SenderTitle;
        }

        public void Send(List<EmailDetails> emails)
        {

            SmtpClient smtp = new SmtpClient(_smtpAddress);
            smtp.EnableSsl = _enableSsl;
            smtp.Port = _port;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_senderAddress, _senderPwd);

            foreach (var email in emails)
            {

                MailMessage message = new MailMessage();
                message.Sender = new MailAddress(_senderAddress, _senderTitle);
                message.From = new MailAddress(_senderAddress, _senderTitle);

                message.To.Add(new MailAddress(email.AddressTo, email.AddressTo));

                message.Subject = email.Subject;
                message.Body = email.Body;

                message.IsBodyHtml = true;
                smtp.Send(message);
            }
        }
    }
}