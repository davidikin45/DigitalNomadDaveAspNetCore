using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.Questions.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IQuestionApplicationService : IBaseEntityApplicationService<QuestionDto, QuestionDto, QuestionDto, QuestionDeleteDto>
    {

    }
}
