using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.MailingLists.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IMailingListApplicationService : IApplicationServiceEntity<MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto>
    {
    }
}
