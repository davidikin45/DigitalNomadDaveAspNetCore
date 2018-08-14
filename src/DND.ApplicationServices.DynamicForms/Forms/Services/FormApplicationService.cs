using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.DynamicForms.Forms.Services
{
    public class FormApplicationService : BaseEntityApplicationService<Form, FormDto, FormDto, FormDto, FormDeleteDto, IFormDomainService>, IFormApplicationService
    {
        public FormApplicationService(IFormDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<FormDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }

        public Task<FormDto> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}