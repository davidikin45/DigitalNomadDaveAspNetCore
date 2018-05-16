using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Email
{
    public class EmailServiceOptions
    {
        public string FromDisplayName { get; set; }
        public string FromEmail { get; set; }

        public string ToEmail { get; set; }
        public string ToDisplayName { get; set; }

        //Write to Disk
        public bool WriteEmailsToFileSystem { get; set; }
        public string PickupDirectoryLocation { get; set; }

        //Smtp
        public bool SendEmailsViaSmtp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public Int32 Port { get; set; }
        public bool Ssl { get; set; }
    }
}
