using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.ContentTexts.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IContentTextApplicationService : IBaseEntityApplicationService<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto>
    {
    }
}
