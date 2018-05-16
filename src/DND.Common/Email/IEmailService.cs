using DND.Common.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Email
{
    public interface IEmailService
    {
        Task<Result> SendEmailMessageAsync(EmailMessage message);
        Task<Result> SendEmailAsync(string email, string subject, string message);
        bool SendEmailMessages(IList<EmailMessage> messages);
        Task<Result> SendEmailMessageToAdminAsync(EmailMessage message);
    }
}
