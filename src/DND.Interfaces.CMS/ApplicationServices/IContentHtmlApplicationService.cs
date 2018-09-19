using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.ContentHtmls.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IContentHtmlApplicationService : IApplicationServiceEntity<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto>
    {
    }
}
