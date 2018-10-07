using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Authors.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Authors.Services
{
    public class AuthorApplicationService : ApplicationServiceEntityBase<Author, AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorDomainService>, IAuthorApplicationService
    {
        public AuthorApplicationService(IAuthorDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<AuthorDto>> hubContext)
        : base("blog.authors.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }

        public async Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAuthorAsync(authorSlug, cancellationToken);
            return Mapper.Map<AuthorDto>(bo);
        }

    }
}