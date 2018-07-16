﻿using DND.Common.DomainEvents;
using DND.Common.Implementation.Validation;
using DND.Domain.Blog.Tags;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class TagInsertedEventHandler : IDomainEventHandler<EntityInsertedEvent<Tag>>
    {
        public IDictionary<string, string> HandleActions => new Dictionary<string, string>(){ { "SendEmail", "Send Email" } };

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
