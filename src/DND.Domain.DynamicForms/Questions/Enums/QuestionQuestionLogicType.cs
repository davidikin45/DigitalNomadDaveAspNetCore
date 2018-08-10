using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions.Enums
{
    public enum QuestionQuestionLogicType
    {
        [Display(Name = "Show question once when answer contains")]
        ShowAnswerOnceContains,
        [Display(Name = "Show question once when answer does not contain")]
        ShowAnswerOnceNotContains
    }
}
