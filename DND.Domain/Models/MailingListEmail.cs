using Solution.Base.ModelMetadataCustom;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
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
