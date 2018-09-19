using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;

namespace DND.Common.Domain
{
    public abstract class EntityAuditableBase<T> : EntityBase<T>, IEntityAuditable where T : IEquatable<T>
    {
        private DateTime? _dateCreated;
        public virtual DateTime DateCreated
        {
            get { return _dateCreated ?? DateTime.UtcNow; }
            set { _dateCreated = value; }
        }
        public string UserOwner { get; set; }

        public string UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }

        //[NotMapped]
        public DateTime? DateDeleted { get; set; }
        //[NotMapped]
        public string UserDeleted { get; set; }
    }
}
