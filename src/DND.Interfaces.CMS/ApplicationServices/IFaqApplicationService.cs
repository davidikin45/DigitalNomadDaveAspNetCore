using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.Faqs.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IFaqApplicationService : IApplicationServiceEntity<FaqDto, FaqDto, FaqDto, FaqDeleteDto>
    {
    }
}
