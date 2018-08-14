using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.Forms.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IFormApplicationService : IBaseEntityApplicationService<FormDto, FormDto, FormDto, FormDeleteDto>
    {
        Task<FormDto> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken);
    }
}
