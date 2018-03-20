using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Authors.Services
{
    public class AuthorApplicationService : BaseEntityApplicationService<IApplicationDbContext, Author, AuthorDto, IAuthorDomainService>, IAuthorApplicationService
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