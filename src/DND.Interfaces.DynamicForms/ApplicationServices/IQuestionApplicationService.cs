using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.Questions.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IQuestionApplicationService : IApplicationServiceEntity<QuestionDto, QuestionDto, QuestionDto, QuestionDeleteDto>
    {

    }
}
