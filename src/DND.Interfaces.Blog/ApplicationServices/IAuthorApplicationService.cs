using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.Blog.Authors.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface IAuthorApplicationService : IApplicationServiceEntity<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto>
    {
        Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
