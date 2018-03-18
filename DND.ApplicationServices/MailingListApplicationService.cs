using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Interfaces.Persistance;

namespace DND.ApplicationServices
{
    public class MailingListApplicationService : BaseEntityApplicationService<IBaseDbContext, MailingList, MailingListDTO, IMailingListDomainService>, IMailingListApplicationService
    {
        public MailingListApplicationService(IMailingListDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
