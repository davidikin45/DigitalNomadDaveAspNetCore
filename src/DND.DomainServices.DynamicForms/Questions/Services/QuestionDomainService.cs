using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data.DynamicForms;
using DND.Domain.DynamicForms.Questions;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.Questions.Services
{
    public class QuestionDomainService : DomainServiceEntityBase<DynamicFormsContext, Question>, IQuestionDomainService
    {
        public QuestionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
