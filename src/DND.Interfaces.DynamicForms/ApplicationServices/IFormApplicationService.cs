using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.Forms.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IFormApplicationService : IApplicationServiceEntity<FormDto, FormDto, FormDto, FormDeleteDto>
    {
        Task<FormDto> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken);
    }
}
