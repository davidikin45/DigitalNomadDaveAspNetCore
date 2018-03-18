using DND.Domain.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IAuthorApplicationService : IBaseEntityApplicationService<AuthorDTO>
    {
        Task<AuthorDTO> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
