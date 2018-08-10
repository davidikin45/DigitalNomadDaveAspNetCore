using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.DynamicForms.Questions.Dtos;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.Questions.Controllers
{
    [Route("admin/forms/questions")]
    public class AdminQuestionsController : BaseEntityControllerAuthorize<QuestionDto, QuestionDto, QuestionDto, QuestionDeleteDto, IQuestionApplicationService>
    {
        public AdminQuestionsController(IQuestionApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
