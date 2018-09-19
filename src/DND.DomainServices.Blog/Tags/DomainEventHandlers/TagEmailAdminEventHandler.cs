using DND.Common.Infrastructure.DomainEvents;
using DND.Common.Infrastructure.Interfaces.DomainEvents;
using DND.Common.Infrastructure.Validation;
using DND.Domain.Blog.Tags;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class TagEmailAdminEventHandler : IDomainEventHandler<EntityActionEvent<Tag>>
    {
        public IDictionary<string, string> HandleActions => new Dictionary<string, string>(){ { "EmailAdmin", "Email Admin" } };

        public bool HandlePreCommitCondition(EntityActionEvent<Tag> domainEvent)
        {
            return true;
        }

        public async Task<Result> HandlePreCommitAsync(EntityActionEvent<Tag> domainEvent)
        {
            var after = domainEvent.Entity;

            return await Task.FromResult(Result.Ok());
        }

        public bool HandlePostCommitCondition(EntityActionEvent<Tag> domainEvent)
        {
            return true;
        }

        public async Task<Result> HandlePostCommitAsync(EntityActionEvent<Tag> domainEvent)
        {
            var before = domainEvent.Entity;

            return await Task.FromResult(Result.Ok());
        }
    }
}
