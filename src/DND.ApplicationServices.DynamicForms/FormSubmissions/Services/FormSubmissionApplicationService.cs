using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.DynamicForms.FormSubmissions;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.FormSubmissions.Services
{
    public class FormSubmissionApplicationService : ApplicationServiceEntityBase<FormSubmission, FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto, IFormSubmissionDomainService>, IFormSubmissionApplicationService
    {
        public FormSubmissionApplicationService(IFormSubmissionDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<FormSubmissionDto>> hubContext)
        :base("forms.form-submissions.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
