using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public interface IDbContextDomainEvents
    {
        Task FirePreCommitEventsAsync();
        Task FirePostCommitEventsAsync();

        IEnumerable<object> GetNewDeletedEntities();
        IEnumerable<object> GetNewUpdatedEntities();
        IEnumerable<object> GetNewInsertedEntities();
    }
}
