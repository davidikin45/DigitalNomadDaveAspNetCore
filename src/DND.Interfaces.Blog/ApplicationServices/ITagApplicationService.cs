using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.Blog.Tags.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface ITagApplicationService : IBaseEntityApplicationService<TagDto, TagDto, TagDto, TagDeleteDto>
    {
        Task<TagDto> GetTagAsync(string tagSlug, CancellationToken cancellationToken);
    }
}
