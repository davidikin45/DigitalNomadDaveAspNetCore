using DND.Common.DomainEvents;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class CategoryPropertyUpdatedEventHandler : IDomainEventHandler<EntityPropertyUpdatedEvent<Category>>
    {
        public async Task HandlePostCommitAsync(EntityPropertyUpdatedEvent<Category> domainEvent)
        {
            var after = domainEvent.Entity;
        }

        public async Task HandlePreCommitAsync(EntityPropertyUpdatedEvent<Category> domainEvent)
        {
            var before = domainEvent.Entity;
        }
    }
}
