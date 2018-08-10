using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions.Enums
{
    public enum QuestionSectionLogicType
    {
        [Display(Name = "Repeat section based on answer")]
        Repeat,
        [Display(Name = "Show section once when answer contains")]
        ShowAnswerOnceContains,
        [Display(Name = "Show section twice when answer contains")]
        ShowAnswerTwiceContains,
        [Display(Name = "Show section three times when answer contains")]
        ShowAnswerThreeContains,
        [Display(Name = "Show Section once when answer does not contain")]
        ShowAnswerOnceNotContains,
        [Display(Name = "Show Section twice when answer does not contain")]
        ShowAnswerTwiceNotContains,
        [Display(Name = "Show Section three times when answer does not contain")]
        ShowAnswerThreeNotContains
    }
}
