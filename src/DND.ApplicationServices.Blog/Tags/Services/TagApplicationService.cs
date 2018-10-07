using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.Blog.Tags;
using DND.Domain.Blog.Tags.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Tags.Services
{
    public class TagApplicationService : ApplicationServiceEntityBase<Tag, TagDto, TagDto, TagDto, TagDeleteDto, ITagDomainService>, ITagApplicationService
    {
        public TagApplicationService(ITagDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<TagDto>> hubContext)
        : base("blog.tags.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }

        public async Task<TagDto> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetTagAsync(tagSlug, cancellationToken);
            return Mapper.Map<TagDto>(bo);
        }
    }
}