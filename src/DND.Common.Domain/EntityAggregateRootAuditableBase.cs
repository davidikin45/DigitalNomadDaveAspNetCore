using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;

namespace DND.Common.Domain
{
    public abstract class EntityAggregateRootAuditableBase<T> : EntityAggregateRootBase<T>, IEntityAuditable where T : IEquatable<T>
    {
        public virtual DateTime DateCreated { get; set; }

        public string UserOwner { get; set; }

        public string UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }

        public DateTime? DateDeleted { get; set; }
        public string UserDeleted { get; set; }

    }
}
