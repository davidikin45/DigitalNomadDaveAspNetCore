using DND.Domain.Models;
using DND.Common.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Domain.Blog.BlogPosts;

namespace DND.DomainServices.Blog.BlogPosts.DomainEventHandlers
{
    public class BlogPostInsertEventHandler : IDomainEventHandler<EntityInsertedEvent<BlogPost>>
    {
        public async Task HandlePostCommitAsync(EntityInsertedEvent<BlogPost> domainEvent)
        {
            var after = domainEvent.Entity;
        }

        public async Task HandlePreCommitAsync(EntityInsertedEvent<BlogPost> domainEvent)
        {
            var before = domainEvent.Entity;
        }
    }
}
