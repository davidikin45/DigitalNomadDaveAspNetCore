using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Questions;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.Questions.Services
{
    public class QuestionDomainService : BaseEntityDomainService<IDynamicFormsDbContext, Question>, IQuestionDomainService
    {
        public QuestionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
