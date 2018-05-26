using DND.Common.DomainEvents;
using DND.Common.Implementation.Validation;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Tags;
using DND.Domain.Interfaces.DomainServices;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class TagInsertedEventHandler : IDomainEventHandler<EntityInsertedEvent<Tag>>
    {

        public bool HandlePreCommitCondition(EntityInsertedEvent<Tag> domainEvent)
        {
            return true;
        }

        public async Task<Result> HandlePreCommitAsync(EntityInsertedEvent<Tag> domainEvent)
        {
            var after = domainEvent.Entity;

            return Result.Ok();
        }

        public bool HandlePostCommitCondition(EntityInsertedEvent<Tag> domainEvent)
        {
            return true;
        }

        public async Task<Result> HandlePostCommitAsync(EntityInsertedEvent<Tag> domainEvent)
        {
            var before = domainEvent.Entity;

            return Result.Ok();
        }
    }
}
