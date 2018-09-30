using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.Questions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.Questions.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/questions")]
    public class QuestionsController : ApiControllerEntityAuthorizeBase<QuestionDto, QuestionDto, QuestionDto, QuestionDeleteDto, IQuestionApplicationService>
    {
        public QuestionsController(IQuestionApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, urlHelper, typeHelperService, appSettings)
        {

        }
    }
}
