using DND.Domain.Models;
using Solution.Base.Interfaces.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface IAuthorDomainService : IBaseEntityDomainService<Author>
    {
        Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
