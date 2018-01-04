using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Email
{
    public interface IEmailService
    {
        bool SendEmailMessage(EmailMessage message);
        bool SendEmailMessages(IList<EmailMessage> messages);
        bool SendEmailMessageToAdmin(EmailMessage message);
    }
}
