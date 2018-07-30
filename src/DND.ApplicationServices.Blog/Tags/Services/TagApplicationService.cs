using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.Blog.Tags;
using DND.Domain.Blog.Tags.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Tags.Services
{
    public class TagApplicationService : BaseEntityApplicationService<Tag, TagDto, TagDto, TagDto, TagDeleteDto, ITagDomainService>, ITagApplicationService
    {
        public TagApplicationService(ITagDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }

        public async Task<TagDto> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetTagAsync(tagSlug, cancellationToken);
            return Mapper.Map<TagDto>(bo);
        }
    }
}