using DND.Domain.Models;
using Solution.Base.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.DomainEventHandlers
{
    public class BlogPostInsertEventHandler : IDomainEventHandler<InsertEntityEvent<BlogPost>>
    {
        public void HandlePostCommit(InsertEntityEvent<BlogPost> domainEvent)
        {
            var after = domainEvent.Entity;
        }

        public void HandlePreCommit(InsertEntityEvent<BlogPost> domainEvent)
        {
            var before = domainEvent.Entity;
        }
    }
}
