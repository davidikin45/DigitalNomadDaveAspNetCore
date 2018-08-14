﻿using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System.ComponentModel.DataAnnotations;

namespace DND.Web.CMS.MVCImplementation.MailingList.Models
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