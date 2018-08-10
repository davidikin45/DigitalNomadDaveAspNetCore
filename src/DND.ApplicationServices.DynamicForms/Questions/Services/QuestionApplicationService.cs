using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Questions.Dtos;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.Questions.Services
{
    public class QuestionApplicationService : BaseEntityApplicationService<Question, QuestionDto, QuestionDto, QuestionDto, QuestionDeleteDto, IQuestionDomainService>, IQuestionApplicationService
    {
        public QuestionApplicationService(IQuestionDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<QuestionDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
    }
}
