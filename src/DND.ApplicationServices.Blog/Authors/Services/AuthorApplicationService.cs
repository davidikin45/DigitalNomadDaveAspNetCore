using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Authors.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Authors.Services
{
    public class AuthorApplicationService : BaseEntityApplicationService<Author, AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorDomainService>, IAuthorApplicationService
    {
        public AuthorApplicationService(IAuthorDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<AuthorDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }

        public async Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAuthorAsync(authorSlug, cancellationToken);
            return Mapper.Map<AuthorDto>(bo);
        }

    }
}