using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Authors.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Authors.Services
{
    public class AuthorApplicationService : BaseEntityApplicationService<Author, AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorDomainService>, IAuthorApplicationService
    {
        public AuthorApplicationService(IAuthorDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }

        public async Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAuthorAsync(authorSlug, cancellationToken);
            return Mapper.Map<AuthorDto>(bo);
        }

    }
}