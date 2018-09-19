using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.ContentTexts.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IContentTextApplicationService : IApplicationServiceEntity<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto>
    {
    }
}
