using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Base.Implementation.Models
{
    public class LogAction : BaseEntity<int>
    { 
        public LogAction(string username, string controller, string action, string description, DateTime perfomedAt, string ip)
        {
            Username = username;
            Action = action;
            Controller = controller;
            Description = description;
            PerformedAt = perfomedAt;
            IP = ip;
        }

        public DateTime PerformedAt { get; private set; }

        public string Controller { get; private set; }

        public string Action { get; private set; }

        public string Username { get; private set; }

        public string IP { get; private set; }

        public string Description { get; private set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
