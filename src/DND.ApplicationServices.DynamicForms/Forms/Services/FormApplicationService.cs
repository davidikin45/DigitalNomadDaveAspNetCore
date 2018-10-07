using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.DynamicForms.Forms.Services
{
    public class FormApplicationService : ApplicationServiceEntityBase<Form, FormDto, FormDto, FormDto, FormDeleteDto, IFormDomainService>, IFormApplicationService
    {
        public FormApplicationService(IFormDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<FormDto>> hubContext)
        : base("forms.forms.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }

        public Task<FormDto> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken)
        {
            return GetOneAsync(cancellationToken, f => f.UrlSlug == formUrlSlug, true, true);
        }
    }
}