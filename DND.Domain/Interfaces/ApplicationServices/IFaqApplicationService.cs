using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.Faqs.Dtos;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IFaqApplicationService : IBaseEntityApplicationService<FaqDto, FaqDto, FaqDto, FaqDto>
    {
    }
}
