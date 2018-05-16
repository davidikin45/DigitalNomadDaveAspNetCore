using Hangfire;
using DND.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DND.Common.Implementation.Validation;
using Microsoft.Extensions.Options;

namespace DND.Common.Email
{
    public class EmailServiceSmtp : EmailServiceFileSystem
    {
        private EmailServiceFileSystem EmailServiceFileSystem;
        public EmailServiceSmtp(IOptions<EmailServiceOptions> options)
            :base(options)
        {
            EmailServiceFileSystem = new EmailServiceFileSystem(options);
        }

        public async override Task<Result> SendEmailMessageAsync(EmailMessage message)
        {
            //Send Smtp Email
            var result = await base.SendEmailMessageAsync(message);

            //May also want to save a copy to disk
            if(result.IsSuccess && Options.WriteEmailsToFileSystem)
            {
                await EmailServiceFileSystem.SendEmailMessageAsync(message);
            }

            return result;
        }

        public override SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = Options.Host,
                Port = Options.Port,
                EnableSsl = Options.Ssl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Options.Username, Options.Password)
            };
        }
    }
}
