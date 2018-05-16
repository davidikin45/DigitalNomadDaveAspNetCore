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
using System.IO;

namespace DND.Common.Email
{
    public class EmailServiceFileSystem : IEmailService
    {
        protected EmailServiceOptions Options;

        public EmailServiceFileSystem(IOptions<EmailServiceOptions> options)
        {
            Options = options.Value;
        }

        public async Task<Result> SendEmailMessageToAdminAsync(EmailMessage message)
        {
            message.ToEmail = Options.ToEmail;
            message.ToDisplayName = Options.ToDisplayName;
            return await SendEmailMessageAsync(message).ConfigureAwait(false);
        }

        public async Task<Result> SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new EmailMessage
            {
                ToEmail = email,
                Subject = subject,
                Body = message,
            };

            return await SendEmailMessageAsync(emailMessage).ConfigureAwait(false);
        }

        public virtual async Task<Result> SendEmailMessageAsync(EmailMessage message)
        {
            try
            {
                if(Options.WriteEmailsToFileSystem)
                {
                    using (var smtp = CreateSmtpClient())
                    {
                        var from = new MailAddress(Options.FromEmail, Options.FromDisplayName);
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

                            await smtp.SendMailAsync(smtpMessage).ConfigureAwait(false);
                        }
                    }
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                //todo: add logging integration
                //throw;
            }

            return Result.Fail(ErrorType.EmailSendFailed);
        }

        public virtual SmtpClient CreateSmtpClient()
        {
            if (!Directory.Exists(Options.PickupDirectoryLocation)) Directory.CreateDirectory(Options.PickupDirectoryLocation);

            return new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = Options.PickupDirectoryLocation
            };
        }

        public bool SendEmailMessages(IList<EmailMessage> messages)
        {
            foreach (EmailMessage message in messages)
            {
                BackgroundJob.Enqueue(() => SendEmailMessageAsync(message));
            }
            return true;
        }
    }
}
