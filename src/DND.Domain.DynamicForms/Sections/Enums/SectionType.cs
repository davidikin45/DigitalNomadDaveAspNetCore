using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Sections.Enums
{
    public enum SectionType
    {
        [Display(Name = "Questions")]
        Questions,
        [Display(Name = "Submission Confirmation")]
        SubmissionConfirmation
    }
}
