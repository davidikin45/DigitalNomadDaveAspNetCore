using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Domain.Blog.Authors;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.DomainServices
{
    public interface IAuthorDomainService : IDomainServiceEntity<Author>
    {
        Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
