using DND.Common.DomainEvents;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class CategoryPropertyUpdatedEventHandler : IDomainEventHandler<EntityPropertyUpdatedEvent<Category>>
    {
        public void HandlePostCommit(EntityPropertyUpdatedEvent<Category> domainEvent)
        {
            var after = domainEvent.Entity;
        }

        public void HandlePreCommit(EntityPropertyUpdatedEvent<Category> domainEvent)
        {
            var before = domainEvent.Entity;
        }
    }
}
