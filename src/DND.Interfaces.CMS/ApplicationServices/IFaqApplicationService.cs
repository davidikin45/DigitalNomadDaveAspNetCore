using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.Faqs.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IFaqApplicationService : IBaseEntityApplicationService<FaqDto, FaqDto, FaqDto, FaqDeleteDto>
    {
    }
}
