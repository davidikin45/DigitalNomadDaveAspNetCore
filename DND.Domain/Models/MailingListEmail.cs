﻿using DND.Base.ModelMetadata;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Models
{
    public class MailingListEmail
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        [MultilineText(HTML = true, Rows = 7)]
        public string Body { get; set; }
    }
}
