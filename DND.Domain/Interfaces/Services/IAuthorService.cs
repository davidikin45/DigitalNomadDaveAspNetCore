using DND.Domain.DTOs;
using Solution.Base.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.Services
{
    public interface IAuthorService : IBaseEntityService<AuthorDTO>
    {
        Task<AuthorDTO> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
