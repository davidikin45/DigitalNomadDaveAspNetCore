using Solution.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpConfiguration _config;

        private const string EmailFromDisplayNameKey = "EmailFromDisplayName";
        private const string EmailToDisplayNameKey = "EmailToDisplayName";
        private const string EmailUsernameKey = "EmailUsername";
        private const string EmailFromEmailKey = "EmailFromEmail";
        private const string EmailToEmailKey = "EmailToEmail";
        private const string EmailPasswordKey = "EmailPassword";
        private const string EmailHostKey = "EmailHost";
        private const string EmailPortKey = "EmailPort";
        private const string EmailSslKey = "EmailSsl";

        public EmailService()
        {
            _config = new SmtpConfiguration();

            var fromDisplayName = ConfigurationManager.AppSettings(EmailFromDisplayNameKey);
            var fromEmail = ConfigurationManager.AppSettings(EmailFromEmailKey);
            var username = ConfigurationManager.AppSettings(EmailFromEmailKey);
            var password = ConfigurationManager.AppSettings(EmailPasswordKey);
            var host = ConfigurationManager.AppSettings(EmailHostKey);
            var port = Int32.Parse(ConfigurationManager.AppSettings(EmailPortKey));
            var ssl = Boolean.Parse(ConfigurationManager.AppSettings(EmailSslKey));

            _config.FromDisplayName = fromDisplayName;
            _config.FromEmail = fromEmail;
            _config.Username = username;
            _config.Password = password;
            _config.Host = host;
            _config.Port = port;
            _config.Ssl = ssl;
        }

        public bool SendEmailMessageToAdmin(EmailMessage message)
        {
            message.ToEmail = ConfigurationManager.AppSettings(EmailToEmailKey);
            message.ToDisplayName = ConfigurationManager.AppSettings(EmailToDisplayNameKey);
            return SendEmailMessage(message);
        }

        public bool SendEmailMessage(EmailMessage message)
        {
            var success = false;
            try
            {
                var smtp = new SmtpClient
                {
                    Host = _config.Host,
                    Port = _config.Port,
                    EnableSsl = _config.Ssl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_config.Username, _config.Password)
                };

                var from = new MailAddress(_config.FromEmail, _config.FromDisplayName);
                var to = new MailAddress(message.ToEmail, message.ToDisplayName);

                using (var smtpMessage = new MailMessage(from, to))
                {
                    smtpMessage.Subject = message.Subject;
                    smtpMessage.SubjectEncoding = Encoding.UTF8;
                    smtpMessage.Body = message.Body;
                    smtpMessage.IsBodyHtml = message.IsHtml;
                    smtpMessage.BodyEncoding = Encoding.UTF8;

                    if (!string.IsNullOrEmpty(message.ReplyEmail))
                    {
                        smtpMessage.ReplyToList.Add(new MailAddress(message.ReplyEmail, message.ReplyDisplayName));
                    }

                    smtp.Send(smtpMessage);
                }

                success = true;
            }
            catch (Exception ex)
            {
                //todo: add logging integration
                //throw;
            }

            return success;
        }

        public bool SendEmailMessages(IList<EmailMessage> messages)
        {
           foreach(EmailMessage message in messages)
            {
                //BackgroundJob.Enqueue(() => SendEmailMessage(message));
            }
            return true;
        }
    }
}
