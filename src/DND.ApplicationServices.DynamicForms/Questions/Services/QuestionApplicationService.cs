using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Questions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.Questions.Services
{
    public class QuestionApplicationService : ApplicationServiceEntityBase<Question, QuestionDto, QuestionDto, QuestionDto, QuestionDeleteDto, IQuestionDomainService>, IQuestionApplicationService
    {
        public QuestionApplicationService(IQuestionDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<QuestionDto>> hubContext)
        : base("forms.questions.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
