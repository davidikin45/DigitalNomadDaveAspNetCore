using DND.Common.Extensions;
using DND.Common.Implementation.Persistance.InMemory;
using DND.Common.Interfaces.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public class DbContextDomainEventsInMemoryAdapter : BaseDbContextDomainEvents
    {
        private InMemoryDataContext _dbContext;
        public DbContextDomainEventsInMemoryAdapter(InMemoryDataContext dbContext, IDomainEvents domainEvents)
            : base(domainEvents)
        {
            _dbContext = dbContext;
        }

        protected override IEnumerable<object> GetDeletedEntities()
        {
            return _dbContext.removeQueue.Select(x => x.Entity);
        }

        protected override IEnumerable<object> GetInsertedEntities()
        {
            return _dbContext.addQueue.Select(x => x.Entity);
        }

        protected override IEnumerable<object> GetUpdatedEntities()
        {
            return _dbContext.updateQueue.Select(x => x.Entity);
        }

        protected override IEnumerable<object> GetUpdatedDeletedInsertedEntities()
        {
            return _dbContext.updateQueue.Select(x => x.Entity).Concat(_dbContext.removeQueue.Select(x => x.Entity)).Concat(_dbContext.addQueue.Select(x => x.Entity));
        }

        protected override Dictionary<object, List<IDomainEvent>> GetNewPropertyUpdatedEvents()
        {
            return new Dictionary<object, List<IDomainEvent>>();
        }
    }
}
