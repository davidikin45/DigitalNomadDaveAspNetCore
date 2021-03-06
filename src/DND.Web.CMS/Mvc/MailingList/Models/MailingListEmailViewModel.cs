﻿using DND.Common.Domain.ModelMetadata;
using System.ComponentModel.DataAnnotations;

namespace DND.Web.CMS.Mvc.MailingList.Models
{
    public class MailingListEmailViewModel
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        [MultilineText(HTML = true, Rows = 7)]
        public string Body { get; set; }
    }
}
