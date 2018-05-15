using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.MailingLists.Dtos;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IMailingListApplicationService : IBaseEntityApplicationService<MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto>
    {
    }
}
