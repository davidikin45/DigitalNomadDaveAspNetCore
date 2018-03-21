using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.Blog.Authors.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IAuthorApplicationService : IBaseEntityApplicationService<AuthorDto, AuthorDto, AuthorDto, AuthorDto>
    {
        Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
