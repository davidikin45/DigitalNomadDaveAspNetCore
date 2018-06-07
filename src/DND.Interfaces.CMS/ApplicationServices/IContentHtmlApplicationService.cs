using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.ContentHtmls.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IContentHtmlApplicationService : IBaseEntityApplicationService<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto>
    {
    }
}
